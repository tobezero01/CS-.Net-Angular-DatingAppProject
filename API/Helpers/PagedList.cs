using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        // Constructor khởi tạo danh sách phân trang với các tham số:
        // items: danh sách phần tử hiện tại của trang
        // count: tổng số phần tử của nguồn dữ liệu
        // pageNumber: số trang hiện tại
        // pageSize: số phần tử trên mỗi trang
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            // Gán số trang hiện tại
            CurrentPage = pageNumber;
            // Tính tổng số trang: làm tròn lên kết quả của phép chia tổng số phần tử cho số phần tử trên 1 trang
            TotalPages = (int) Math.Ceiling(count / (double)pageSize);
            // Gán số phần tử trên mỗi trang
            PageSize = pageSize;
            // Gán tổng số phần tử
            TotalCount = count;
            // Thêm các phần tử vào danh sách (lớp cha List<T>)
            AddRange(items);
        }

        // Thuộc tính lưu số trang hiện tại
        public int CurrentPage { get; set; }
        // Thuộc tính lưu tổng số trang
        public int TotalPages { get; set; }
        // Thuộc tính lưu số phần tử trên mỗi trang
        public int PageSize { get; set; }
        // Thuộc tính lưu tổng số phần tử
        public int TotalCount { get; set; }

        // Phương thức tĩnh bất đồng bộ để tạo một PagedList từ một nguồn IQueryable<T>
        // source: nguồn dữ liệu dạng IQueryable (ví dụ truy vấn từ Entity Framework)
        // pageNumber: số trang cần lấy
        // pageSize: số phần tử trên mỗi trang
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // Đếm tổng số phần tử trong nguồn dữ liệu (bất đồng bộ)
            var count = await source.CountAsync();
            // Lấy các phần tử của trang hiện tại:
            // Bỏ qua ((pageNumber - 1) * pageSize) phần tử và sau đó lấy pageSize phần tử (bất đồng bộ)
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            // Trả về một instance của PagedList với các thông tin đã tính toán
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }

}