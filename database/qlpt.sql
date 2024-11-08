-- Tạo bảng Room_Status
CREATE TABLE Room_Status (
    room_status_id INT PRIMARY KEY IDENTITY(1,1),
    status_name NVARCHAR(50) NOT NULL
);

-- Tạo bảng Room_Type
CREATE TABLE Room_Type (
    room_type_id INT PRIMARY KEY IDENTITY(1,1),
    type_name NVARCHAR(50) NOT NULL
);

-- Tạo bảng Utility
CREATE TABLE Utility (
    utility_id INT PRIMARY KEY IDENTITY(1,1),
    utility_name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255)
);

-- Tạo bảng Room_Post
CREATE TABLE Room_Post (
    post_id INT PRIMARY KEY IDENTITY(1,1),
    room_name NVARCHAR(100),
    quantity INT,
    room_price DECIMAL(18,2),
    room_size DECIMAL(18,2),
    address NVARCHAR(255),
    room_description NVARCHAR(MAX),
    date_posted DATE,
    expiration_date DATE,
    status_id INT,
    user_id INT,
    room_type_id INT,
	room_coordinate_id INT,
    FOREIGN KEY (status_id) REFERENCES Room_Status(room_status_id),
    FOREIGN KEY (room_type_id) REFERENCES Room_Type(room_type_id)
);

-- Tạo bảng Room_Utility
CREATE TABLE Room_Utility (
    room_utility_id INT PRIMARY KEY IDENTITY(1,1),
    post_id INT,
    utility_id INT,
    FOREIGN KEY (post_id) REFERENCES Room_Post(post_id),
    FOREIGN KEY (utility_id) REFERENCES Utility(utility_id)
);

-- Tạo bảng Image_Type
CREATE TABLE Image_Type (
    type_id INT PRIMARY KEY IDENTITY(1,1),
    type_name NVARCHAR(50) NOT NULL
);

-- Tạo bảng Room_Image
CREATE TABLE Room_Image (
    image_id INT PRIMARY KEY IDENTITY(1,1),
    post_id INT,
    image_type_id INT,
    image_url NVARCHAR(255),
    FOREIGN KEY (post_id) REFERENCES Room_Post(post_id),
    FOREIGN KEY (image_type_id) REFERENCES Image_Type(type_id)
);

-- Tạo bảng Favorite_List
CREATE TABLE Favorite_List (
    favorite_list_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT
);

-- Tạo bảng Favorite_List_Post
CREATE TABLE Favorite_List_Post (
    favorite_list_post_id INT PRIMARY KEY IDENTITY(1,1),
    post_id INT,
    favorite_id INT,
    FOREIGN KEY (post_id) REFERENCES Room_Post(post_id),
    FOREIGN KEY (favorite_id) REFERENCES Favorite_List(favorite_list_id)
);

-- Tạo bảng Feedback_Type
CREATE TABLE Feedback_Type (
    feedback_type_id INT PRIMARY KEY IDENTITY(1,1),
    type_name NVARCHAR(50),
    description NVARCHAR(255)
);

-- Tạo bảng Feedback
CREATE TABLE Feedback (
    feedback_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    post_id INT,
    status NVARCHAR(50),
    feedback_type_id INT,
    FOREIGN KEY (post_id) REFERENCES Room_Post(post_id),
    FOREIGN KEY (feedback_type_id) REFERENCES Feedback_Type(feedback_type_id)
);

-- Tạo bảng User_Type
CREATE TABLE User_Type (
    user_type_id INT PRIMARY KEY IDENTITY(1,1),
    type_name NVARCHAR(50) NOT NULL
);

-- Tạo bảng User
CREATE TABLE [User] (
    user_id INT PRIMARY KEY IDENTITY(1,1),	
    username NVARCHAR(50) NOT NULL,
	fullname NVARCHAR(255),
	dob DATETIME,
    [password] NVARCHAR(50) NOT NULL,
    address NVARCHAR(255),
    email NVARCHAR(100),
    phone NVARCHAR(15),
    gender NVARCHAR(10),
    user_image NVARCHAR(255),
    user_type_id INT,
	resetToken VARCHAR(100),
	tokenCreateAt DATETIME,
    FOREIGN KEY (user_type_id) REFERENCES User_Type(user_type_id)
);

-- Tạo bảng Page_Address
CREATE TABLE Page_Address (
    page_address_id INT PRIMARY KEY IDENTITY(1,1),
    page_name NVARCHAR(100) NOT NULL
);

-- Tạo bảng Privilege
CREATE TABLE Privilege (
    privilege_id INT PRIMARY KEY IDENTITY(1,1),
    user_type_id INT,
    page_address_id INT,
    is_privileged BIT,
    FOREIGN KEY (user_type_id) REFERENCES User_Type(user_type_id),
    FOREIGN KEY (page_address_id) REFERENCES Page_Address(page_address_id)
);

CREATE TABLE Room_Coordinates (
    room_coordinate_id INT PRIMARY KEY IDENTITY(1,1),
    latitude FLOAT NOT NULL,
    longitude FLOAT NOT NULL
);

CREATE TABLE Service (
	service_id INT PRIMARY KEY IDENTITY(1,1),
	service_name NVARCHAR(50) NOT NULL,
	service_description NVARCHAR(255),
	service_price DECIMAL(18,2),
	service_time INT
);

INSERT INTO Service (service_name, service_description,service_price,service_time)
VALUES 
(N'Tin VIP Nổi Bật', N'Tin được hiển thị nổi bật đầu trang',2000,30),
(N'Tin VIP Độc Đáo', N'Tin được In Hoa màu đỏ nổi bật',1500,30),
(N'Tin VIP Sang Trọng', N'Tin được In Hoa màu xanh sang trọng',1000,30)

CREATE TABLE Bill (
	bill_id INT PRIMARY KEY IDENTITY(1,1),
	post_id INT NOT NULL,
	service_id INT NOT NULL,
	total_Price DECIMAL(18,2),
	payment_date DATE,
	bill_status INT,
	expiration_date DATE,
	FOREIGN KEY (post_id) REFERENCES Room_Post(post_id),
	FOREIGN KEY (service_id) REFERENCES Service(service_id),
);

INSERT INTO Room_Post (room_name, quantity, room_price, room_size, address, room_description, date_posted, expiration_date, status_id, user_id, room_type_id)
VALUES 
(N'Phòng duplex Quận 3', 2, 8000000.00, 45.00, N'456 Nguyễn Thị Minh Khai, Phường 6, Quận 3', N'Phòng duplex sang trọng tại 456 Nguyễn Thị Minh Khai, Quận 3, diện tích 45.00m2, giá thuê 8000000.00 VND. Thích hợp cho 2 người.', '2023-08-15', '2024-08-15', 1, 14, 2),
(N'Studio Quận 3', 1, 10000000.00, 50.00, N'789 Lê Văn Sỹ, Phường 13, Quận 3', N'Studio hiện đại tại 789 Lê Văn Sỹ, Quận 3, diện tích 50.00m2, giá thuê 10000000.00 VND. Phòng lý tưởng cho 1 người.', '2023-07-10', '2024-07-10', 1, 14, 3),
(N'Phòng trọ Quận 1', 1, 5500000.00, 35.00, N'101 Nguyễn Đình Chiểu, Phường Đa Kao, Quận 1', N'Phòng trọ tại 101 Nguyễn Đình Chiểu, Quận 1, diện tích 35.00m2, giá thuê 5500000.00 VND. Phòng phù hợp cho 1 người.', '2024-01-01', '2025-01-01', 1, 14, 1);

INSERT INTO Room_Image (post_id,image_url)
VALUES 
(24,'room_001.jpg'),
(25,'room_002.jpg'),
(26,'room_003.jpg');

ALTER TABLE Room_Post
ADD CONSTRAINT FK_RoomPost_Coordinates
FOREIGN KEY (room_coordinate_id) REFERENCES Room_Coordinates(room_coordinate_id);


SELECT 
    kc.name AS ConstraintName
FROM 
    sys.key_constraints AS kc
JOIN 
    sys.tables AS t ON kc.parent_object_id = t.object_id
WHERE 
    kc.type = 'PK' AND t.name = 'Room_Coordinates';


SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME = 'Room_Post';


alter table Favorite_List
add constraint FK_FavoriteList_User
foreign key (user_id) references [User](user_id)

INSERT INTO Room_Status (status_name)
VALUES (N'Có sẵn'), (N'Đã thuê'), (N'Hết hạn');

INSERT INTO Room_Type (type_name)
VALUES (N'Phòng trọ'), (N'Phòng duplex'), (N'Studio'), (N'Căn hộ');

INSERT INTO Utility (utility_name, description)
VALUES 
(N'Wi-Fi', N'Kết nối internet tốc độ cao'),
(N'Điều hòa', N'Hệ thống làm mát và sưởi ấm'),
(N'Chỗ đậu xe', N'Khu vực đậu xe riêng'),
(N'Giặt là', N'Dịch vụ giặt là trong tòa nhà'),
(N'Thang máy', N'Hệ thống thang máy hiện đại'),
(N'Bảo vệ 24/7', N'Dịch vụ bảo vệ an ninh 24/7'),
(N'Hồ bơi', N'Hồ bơi ngoài trời và trong nhà'),
(N'Phòng gym', N'Phòng tập thể dục đầy đủ trang thiết bị'),
(N'Sân thượng', N'Khu vực sân thượng để thư giãn và tổ chức tiệc'),
(N'Sân vườn', N'Khu vườn chung rộng rãi'),
(N'Cửa hàng tiện lợi', N'Cửa hàng tiện lợi trong khuôn viên tòa nhà'),
(N'Máy giặt', N'Máy giặt trong căn hộ'),
(N'Truyền hình cáp', N'Kênh truyền hình cáp đa dạng'),
(N'Ban công', N'Ban công riêng trong phòng');


INSERT INTO User_Type (type_name)
VALUES (N'Admin'), (N'Chủ nhà'), (N'Người thuê');

INSERT INTO [User] (username, [password], address, email, phone, gender, user_image, user_type_id)
VALUES 
('admin1', 'matkhau1', N'123 Đường Quản Trị', 'quantri1@example.com', '0909123456', N'Nam', 'quantri1.jpg', 1),
('chunha1', 'matkhau2', N'456 Đường Chủ Nhà', 'chunha1@example.com', '0909876543', N'Nữ', 'chunha1.jpg', 2),
('nguoithue1', 'matkhau3', N'789 Đường Người Thuê', 'nguoithue1@example.com', '0912345678', N'Nam', 'nguoithue1.jpg', 3);

INSERT INTO Room_Post (room_name, quantity, room_price, room_size, address, room_description, date_posted, expiration_date, status_id, user_id, room_type_id)
VALUES 
(N'Phòng duplex Quận 3', 2, 8000000.00, 45.00, N'456 Nguyễn Thị Minh Khai, Phường 6, Quận 3', N'Phòng duplex sang trọng tại 456 Nguyễn Thị Minh Khai, Quận 3, diện tích 45.00m2, giá thuê 8000000.00 VND. Thích hợp cho 2 người.', '2023-08-15', '2024-08-15', 1, 2, 2),
(N'Studio Quận 3', 1, 10000000.00, 50.00, N'789 Lê Văn Sỹ, Phường 13, Quận 3', N'Studio hiện đại tại 789 Lê Văn Sỹ, Quận 3, diện tích 50.00m2, giá thuê 10000000.00 VND. Phòng lý tưởng cho 1 người.', '2023-07-10', '2024-07-10', 1, 2, 3),
(N'Phòng trọ Quận 1', 1, 5500000.00, 35.00, N'101 Nguyễn Đình Chiểu, Phường Đa Kao, Quận 1', N'Phòng trọ tại 101 Nguyễn Đình Chiểu, Quận 1, diện tích 35.00m2, giá thuê 5500000.00 VND. Phòng phù hợp cho 1 người.', '2024-01-01', '2025-01-01', 1, 2, 1),
(N'Phòng duplex Quận 3', 2, 8500000.00, 50.00, N'202 Võ Thị Sáu, Phường 1, Quận 3', N'Phòng duplex sang trọng tại 202 Võ Thị Sáu, Quận 3, diện tích 50.00m2, giá thuê 8500000.00 VND. Thích hợp cho 2 người.', '2024-01-15', '2025-01-15', 1, 2, 2),
(N'Studio Quận 10', 1, 12000000.00, 55.00, N'303 Đường Cao Thắng, Phường 12, Quận 10', N'Studio hiện đại tại 303 Đường Cao Thắng, Quận 10, diện tích 55.00m2, giá thuê 12000000.00 VND. Phòng lý tưởng cho 1 người.', '2024-02-01', '2025-02-01', 1, 2, 3),
(N'Phòng trọ Quận 1', 3, 7000000.00, 40.00, N'404 Đường Lê Thị Riêng, Phường Bến Thành, Quận 1', N'Phòng trọ tại 404 Đường Lê Thị Riêng, Quận 1, diện tích 40.00m2, giá thuê 7000000.00 VND. Phòng phù hợp cho 3 người.', '2024-02-15', '2025-02-15', 1, 2, 1),
(N'Phòng duplex Quận 1', 1, 9000000.00, 45.00, N'505 Đường Nguyễn Huệ, Phường Bến Nghé, Quận 1', N'Phòng duplex sang trọng tại 505 Đường Nguyễn Huệ, Quận 1, diện tích 45.00m2, giá thuê 9000000.00 VND. Thích hợp cho 1 người.', '2024-03-01', '2025-03-01', 1, 2, 2);


INSERT INTO Room_Utility (post_id, utility_id)
VALUES 
(1, 1), -- Phòng A có Wi-Fi
(1, 2), -- Phòng A có Điều hòa
(2, 1), -- Phòng B có Wi-Fi
(2, 3), -- Phòng B có Chỗ đậu xe
(3, 4); -- Studio X có Giặt là

INSERT INTO Image_Type (type_name)
VALUES (N'Ảnh bìa'), (N'Ảnh tổng quan tầng'), (N'Ảnh hành lang'), (N'Ảnh phòng cho thuê'), (N'Ảnh tiện ích bên ngoài');

INSERT INTO Room_Image (post_id, image_type_id, image_url)
VALUES 
(1, 1, 'phong_a_hinh_thu_nho.jpg'),
(1, 2, 'phong_a_thu_vien1.jpg'),
(2, 1, 'phong_b_hinh_thu_nho.jpg'),
(2, 2, 'phong_b_thu_vien1.jpg'),
(3, 1, 'studio_x_hinh_thu_nho.jpg');

INSERT INTO Favorite_List (name)
VALUES (N'Danh sách yêu thích của tôi');

INSERT INTO Favorite_List_Post (post_id, favorite_id, user_id, date)
VALUES 
(1, 1, 3, '2023-09-10'),
(2, 1, 3, '2023-09-11');

INSERT INTO Feedback_Type (type_name, description)
VALUES 
(N'Lừa đảo', N'Phản hồi về hành vi lừa đảo hoặc gian lận trong bài đăng'),
(N'Bài đăng không hợp lệ', N'Phản hồi về bài đăng không đúng quy định hoặc vi phạm chính sách'),
(N'Yêu cầu thêm thông tin', N'Người dùng yêu cầu thêm thông tin về phòng trọ hoặc nhà'),
(N'Thông tin không chính xác', N'Phản hồi về thông tin không chính xác trong bài đăng'),
(N'Chất lượng hình ảnh kém', N'Phản hồi về hình ảnh không rõ ràng hoặc chất lượng kém trong bài đăng');

INSERT INTO Feedback (user_id, post_id, status, feedback_type_id)
VALUES 
(3, 1, N'Đã duyệt', 1), 
(3, 2, N'Đang chờ', 2); 

INSERT INTO Page_Address (page_name)
VALUES (N'Bảng điều khiển'), (N'Quản lý bài đăng'), (N'Xem danh sách phòng'), (N'Quản lý người dùng');

INSERT INTO Privilege (user_type_id, page_address_id, is_privileged)
VALUES 
(1, 1, 1), -- Quản trị viên có quyền truy cập Bảng điều khiển
(1, 2, 1), -- Quản trị viên có quyền truy cập Quản lý bài đăng
(1, 3, 1), -- Quản trị viên có quyền truy cập Xem danh sách phòng
(1, 4, 1), -- Quản trị viên có quyền truy cập Quản lý người dùng
(2, 2, 1), -- Chủ nhà có quyền truy cập Quản lý bài đăng
(3, 3, 1); -- Người thuê có quyền truy cập Xem danh sách phòng

select * from Room_Post
select * from Room_Utility
select * from Utility
select * from Room_Type
select * from Room_Image
select * from Image_Type

-- Cập nhật tên phòng với room_type_id = 1 (Phòng trọ)
UPDATE Room_Post
SET room_name = N'Phòng trọ ' + SUBSTRING(address, CHARINDEX(N'Quận', address), LEN(address))
WHERE room_type_id = 1;

-- Cập nhật tên phòng với room_type_id = 2 (Phòng duplex)
UPDATE Room_Post
SET room_name = N'Phòng duplex ' + SUBSTRING(address, CHARINDEX(N'Quận', address), LEN(address))
WHERE room_type_id = 2;

-- Cập nhật tên phòng với room_type_id = 3 (Studio)
UPDATE Room_Post
SET room_name = N'Studio ' + SUBSTRING(address, CHARINDEX(N'Quận', address), LEN(address))
WHERE room_type_id = 3;

-- Cập nhật tên phòng với room_type_id = 4 (Căn hộ)
UPDATE Room_Post
SET room_name = N'Căn hộ ' + SUBSTRING(address, CHARINDEX(N'Quận', address), LEN(address))
WHERE room_type_id = 4;

-- Cập nhật mô tả phòng cho loại phòng trọ
UPDATE Room_Post
SET room_description = N'Phòng trọ tại ' + address + N', diện tích ' + CAST(room_size AS VARCHAR(10)) + N'm2, giá thuê ' + CAST(room_price AS VARCHAR(15)) + N' VND. Phòng phù hợp cho ' + CAST(quantity AS VARCHAR(10)) + N' người.'
WHERE room_type_id = 1;

-- Cập nhật mô tả phòng cho loại phòng duplex
UPDATE Room_Post
SET room_description = N'Phòng duplex sang trọng tại ' + address + N', diện tích ' + CAST(room_size AS VARCHAR(10)) + N'm2, giá thuê ' + CAST(room_price AS VARCHAR(15)) + N' VND. Thích hợp cho ' + CAST(quantity AS VARCHAR(10)) + N' người.'
WHERE room_type_id = 2;

-- Cập nhật mô tả phòng cho loại studio
UPDATE Room_Post
SET room_description = N'Studio hiện đại tại ' + address + N', diện tích ' + CAST(room_size AS VARCHAR(10)) + N'm2, giá thuê ' + CAST(room_price AS VARCHAR(15)) + N' VND. Phòng lý tưởng cho ' + CAST(quantity AS VARCHAR(10)) + N' người.'
WHERE room_type_id = 3;

-- Cập nhật mô tả phòng cho loại căn hộ
UPDATE Room_Post
SET room_description = N'Căn hộ tiện nghi tại ' + address + N', diện tích ' + CAST(room_size AS VARCHAR(10)) + N'm2, giá thuê ' + CAST(room_price AS VARCHAR(15)) + N' VND. Thích hợp cho ' + CAST(quantity AS VARCHAR(10)) + N' người.'
WHERE room_type_id = 4;

select * from Room_Image

insert into Room_Image values
('1', '1', 'room_009.jpg');

insert into Image_Type values
(N'Ảnh khác')

select * from Image_Type

update Room_Image
set image_type_id = 6
where image_id = 11

select * from [User]

SELECT 
    CONSTRAINT_NAME, 
    CONSTRAINT_TYPE
FROM 
    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE 
    TABLE_NAME = 'Favorite_List' AND CONSTRAINT_TYPE = 'FOREIGN KEY';

select * from Favorite_List
select * from Favorite_List_Post

delete from Favorite_List_Post
where favorite_list_post_id = 4

select count(favorite_id) from Favorite_List_Post

select * from Room_Post
select * from Room_Coordinates
select * from [User]

select * from Favorite_List
select * from Favorite_List_Post
select * from Room_Status

select * from Room_Utility
select * from Utility

ALTER TABLE Room_Image
DROP COLUMN image_type_id;
ALTER TABLE Room_Image
DROP CONSTRAINT FK__Room_Imag__image__47DBAE45;

select * from Room_Image
