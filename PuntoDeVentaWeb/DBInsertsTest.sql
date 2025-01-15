INSERT INTO UserRole (Name)
values
('Administrator'),
('Seller');
GO
INSERT INTO Users(Username, Password, Email, Name, UserRoleId)
values
('God', 'admin', 'Admin@administration.com', 'Juan Roman Riquelme', 1),
('Seller1', 'test1', 'Seller1@sales.com', 'Martin Palermo', 2),
('Seller2', 'test2', 'Seller2@sales.com', 'Frodo', 2);

SELECT U.USERNAME, UR.NAME FROM USERS U
INNER JOIN USERROLES UR ON UR.ID = U.UserRoleId 