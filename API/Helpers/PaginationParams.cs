namespace API.Helpers;

// Lớp PaginationParams dùng để lưu trữ các tham số phân trang, như số trang và số phần tử trên mỗi trang.
public class PaginationParams
{
    // Giới hạn số phần tử tối đa trên mỗi trang.
    private const int MaxPageSize = 50;
    
    // Số trang hiện tại, mặc định là 1.
    public int PageNumber { get; set; } = 1;
    
    // Biến lưu trữ số phần tử trên mỗi trang, mặc định là 10.
    private int _pageSize = 10;

    // Thuộc tính PageSize: khi gán giá trị mới, nếu vượt quá MaxPageSize thì sẽ lấy giá trị MaxPageSize.
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
