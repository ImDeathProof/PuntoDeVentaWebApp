USE PointOfSales
GO
INSERT INTO UserRoles (Name)
values
('Administrator'),
('Seller');
GO
INSERT INTO Users(Username, Password, Email, Name, UserRoleId)
values
('God', 'admin', 'Admin@administration.com', 'Juan Roman Riquelme', 1),
('Seller1', 'test1', 'Seller1@sales.com', 'Martin Palermo', 2),
('Seller2', 'test2', 'Seller2@sales.com', 'Frodo', 2);
GO
INSERT INTO PaymentMethods(Name)
VALUES
('Cash'),
('Credit Card'),
('Debit Card'),
('Bank Transfer'),
('Promissory notes'),
('Digital');
GO
INSERT INTO Purchases(SupplierId, UserId, Total, Date, PaymentMethodId)
VALUES(1, 2, 150000.00, GETDATE(), 1);



SELECT U.USERNAME, UR.NAME FROM USERS U
INNER JOIN USERROLES UR ON UR.ID = U.UserRoleId 
select * from Suppliers

