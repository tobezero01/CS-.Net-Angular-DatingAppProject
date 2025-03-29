namespace API.Helpers;

// Lớp PaginationHeader dùng để trả về thông tin phân trang cho phía client, bao gồm số trang hiện tại, 
// số phần tử trên mỗi trang, tổng số phần tử và tổng số trang.
// Định nghĩa dưới dạng "record-like class" với constructor nhận 4 tham số.
public class PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
{
    // Số trang hiện tại
    public int CurrentPage { get; set; } = currentPage;
    
    // Số phần tử trên mỗi trang
    public int ItemsPerPage { get; set; } = itemsPerPage;
    
    // Tổng số phần tử
    public int TotalItems { get; set; } = totalItems;
    
    // Tổng số trang
    public int TotalPages { get; set; } = totalPages;
}