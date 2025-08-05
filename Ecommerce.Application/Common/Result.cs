using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Common
{
    public class Result
    {
        // Thuộc tính cho biết hành động có thành công không.
        // Chỉ có thể set giá trị bên trong class này.
        public bool IsSuccess { get; private set; }

        // Danh sách các lỗi nếu hành động thất bại.
        public IReadOnlyList<string> Errors { get; private set; }

        // Constructor là private để ép người dùng phải sử dụng các phương thức static
        // Success() và Failure(), giúp code trở nên dễ đọc hơn.
        private Result(bool isSuccess, IEnumerable<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors.ToList().AsReadOnly();
        }

        // --- Các phương thức Factory để tạo đối tượng Result ---

        /// <summary>
        /// Tạo một kết quả thành công.
        /// </summary>
        public static Result Success()
        {
            return new Result(true, new List<string>());
        }

        /// <summary>
        /// Tạo một kết quả thất bại với một lỗi duy nhất.
        /// </summary>
        public static Result Failure(string error)
        {
            return new Result(false, new List<string> { error });
        }

        /// <summary>
        /// Tạo một kết quả thất bại với một danh sách các lỗi.
        /// </summary>
        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }
    }
}
