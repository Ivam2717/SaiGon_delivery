# HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY ĐỒ ÁN

**Đề tài:** Xây dựng hệ thống điều phối và giám sát đơn hàng giao nhận nội thành sử dụng công nghệ ASP.NET Core MVC

---

## YÊU CẦU HỆ THỐNG

| Phần mềm | Phiên bản | Ghi chú |
|---|---|---|
| Windows | 10 / 11 | Bắt buộc |
| Visual Studio 2022 | 17.x trở lên | Community Edition (miễn phí) |
| .NET SDK | 8.0 LTS | Cài kèm VS2022 hoặc tải riêng |
| SQL Server | Express 2019/2022 | Miễn phí — cài kèm VS2022 hoặc tải riêng |
| SQL Server Management Studio | 19.x trở lên | Tùy chọn nhưng khuyến nghị |

---

## BƯỚC 1 — TẢI SOURCE CODE

**Cách 1 — Clone từ GitHub:**
```bash
git clone https://github.com/Ivam2717/SaiGon_delivery.git
```

**Cách 2 — Tải file ZIP:**
- Vào https://github.com/Ivam2717/SaiGon_delivery
- Bấm **Code** → **Download ZIP** → Giải nén

---

## BƯỚC 2 — TẠO DATABASE TỪ SCRIPT SQL

### 2.1 Mở SSMS và kết nối SQL Server
- Mở **SQL Server Management Studio (SSMS)**
- Ô **Server name:** nhập `YOUR_PC_NAME\SQLEXPRESS`  
  *(Không biết tên máy: mở Command Prompt → gõ `hostname` → Enter)*
- Authentication: **Windows Authentication**
- Bấm **Connect**

### 2.2 Mở file script SQL
- Trong SSMS, vào menu **File** → **Open** → **File**
- Tìm đến thư mục `Database/` trong source code vừa clone
- Chọn file **`SaigonDelivery_Full.sql`**
- File này chứa đầy đủ: lệnh tạo database, tạo bảng, ràng buộc và toàn bộ dữ liệu mẫu

### 2.3 Chạy script
- Nhấn **F5** hoặc bấm nút **Execute** trên thanh toolbar
- Chờ đến khi thanh trạng thái hiện **"Query executed successfully"**

### 2.4 Kiểm tra kết quả
Chạy câu lệnh sau để xác nhận dữ liệu đã được nạp đúng:

```sql
USE SaigonDeliveryDB;
SELECT Role, COUNT(*) AS SoLuong FROM Users GROUP BY Role;
SELECT COUNT(*) AS TongDon FROM Orders;
SELECT COUNT(*) AS TongDanhGia FROM ShipperRatings;
```

Kết quả mong đợi:
- Admin: 1, Customer: 100, Shipper: 20
- Tổng đơn: 1000
- Tổng đánh giá: ~448

---

## BƯỚC 3 — CẤU HÌNH PROJECT

### 3.1 Mở project
- Vào thư mục vừa clone/giải nén
- Mở file `Saigon_delivery.sln` bằng **Visual Studio 2022**

### 3.2 Cập nhật Connection String
Mở file `appsettings.json`, tìm và sửa dòng sau:

```json
{
  "ConnectionStrings": {
    "SaigonDeliveryDB": "Server=YOUR_PC_NAME\\SQLEXPRESS;Database=SaigonDeliveryDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Thay `YOUR_PC_NAME` bằng tên máy tính của bạn.

> **Cách lấy tên máy:** Mở Command Prompt → gõ `hostname` → Enter

### 3.3 Restore NuGet Packages
Visual Studio tự động restore khi mở solution.
Nếu không tự động: chuột phải vào **Solution** → **Restore NuGet Packages**

---

## BƯỚC 4 — CHẠY PROJECT

- Nhấn **Ctrl + F5** (chạy không debug) hoặc **F5** (chạy có debug)
- Trình duyệt tự động mở tại `https://localhost:[port]/Account/Login`

> **Lưu ý quan trọng:** Sử dụng **HTTPS** (không phải HTTP) vì tính năng bản đồ GPS yêu cầu kết nối bảo mật. Nếu trình duyệt cảnh báo chứng chỉ, chọn **Advanced** → **Proceed** để tiếp tục.

---

## TÀI KHOẢN ĐĂNG NHẬP MẪU

| Vai trò | Email | Mật khẩu |
|---|---|---|
| Admin | admin@saigondelivery.vn | Admin@123 |
| Customer | *(xem bảng Users trong DB)* | Customer@123 |
| Shipper | *(xem bảng Users trong DB)* | Shipper@123 |

**Lấy email Customer/Shipper mẫu** — chạy trong SSMS:
```sql
SELECT TOP 2 Email, FullName, Role FROM Users WHERE Role = 'Customer';
SELECT TOP 2 Email, FullName, Role FROM Users WHERE Role = 'Shipper';
```

---

## CẤU TRÚC THƯ MỤC SOURCE CODE

```
Saigon_delivery/
├── Controllers/          # 6 Controller xử lý nghiệp vụ
│   ├── AccountController.cs
│   ├── OrderController.cs
│   ├── ShipperController.cs
│   ├── AdminController.cs
│   ├── RatingController.cs
│   └── HomeController.cs
├── Models/               # EF Core models + ViewModels
│   ├── User.cs
│   ├── Order.cs
│   ├── ShipperRating.cs
│   └── ViewModels/
├── Data/
│   └── SaigonDeliveryContext.cs   # EF Core DbContext
├── Views/                # 16 Razor Views
├── wwwroot/
│   └── css/site-custom.css        # CSS tùy chỉnh
├── appsettings.json      # Cấu hình connection string
└── Program.cs            # Cấu hình middleware và DI
Database/
├── SaigonDelivery_Schema.sql      # Script tạo bảng
└── SeedData_Realistic_v2.sql      # Dữ liệu mẫu
```

---

## XỬ LÝ LỖI THƯỜNG GẶP

**Lỗi 1: Cannot connect to SQL Server**
- Kiểm tra tên server trong `appsettings.json`
- Đảm bảo SQL Server đang chạy: mở **Services** → tìm `SQL Server (SQLEXPRESS)` → Start

**Lỗi 2: Biểu đồ không hiển thị (Admin/Report)**
- Cần có ít nhất 1 đơn hàng ở trạng thái Completed
- Chạy luồng: Admin gán Shipper → Shipper bấm "Lấy hàng" → "Đã giao"

**Lỗi 3: Bản đồ GPS không hoạt động**
- Chạy trên HTTPS, không phải HTTP
- Cho phép quyền truy cập vị trí khi trình duyệt hỏi

**Lỗi 4: NuGet package lỗi khi build**
- Tools → NuGet Package Manager → Package Manager Console
- Chạy: `Update-Package -reinstall`
