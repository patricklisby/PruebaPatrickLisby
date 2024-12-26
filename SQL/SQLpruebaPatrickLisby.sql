-- Crear la base de datos
CREATE DATABASE pruebaPatrickLisby;
GO

USE pruebaPatrickLisby;
GO

-- Crear la tabla Categorias
CREATE TABLE Categorias (
    idCategoria INT PRIMARY KEY IDENTITY(1,1),
    descripcionCategoria NVARCHAR(255) NOT NULL
);

-- Crear la tabla Permisos
CREATE TABLE Permisos (
    idPermiso INT PRIMARY KEY IDENTITY(1,1),
    descripcionPermiso NVARCHAR(255) NOT NULL
);

-- Crear la tabla Usuarios
CREATE TABLE Usuarios (
    idCedulaUsuario INT PRIMARY KEY,
    nombreUsuario NVARCHAR(100) NOT NULL,
    apellidoUsuario NVARCHAR(100) NOT NULL,
    correoelectronicoUsuario NVARCHAR(255) UNIQUE NOT NULL,
    contrasenaUsuario NVARCHAR(255) NOT NULL,
    idPermiso INT NOT NULL,
    estado INT,
    FOREIGN KEY (idPermiso) REFERENCES Permisos(idPermiso)
);

-- Crear la tabla Productos
CREATE TABLE Productos (
    idProducto INT PRIMARY KEY IDENTITY(1,1),
    descripcionProducto NVARCHAR(255) NOT NULL,
    detallesProducto NVARCHAR(MAX),
    precioProducto DECIMAL(18,2) NOT NULL,
    cantidadProducto INT NOT NULL,
    fechaPublicacion DATE NOT NULL,
    idCategoria INT NOT NULL,
    idCedulaUsuarioRegistra INT NOT NULL,
    estado INT,
    idImagen INT NULL,
    FOREIGN KEY (idCategoria) REFERENCES Categorias(idCategoria),
    FOREIGN KEY (idCedulaUsuarioRegistra) REFERENCES Usuarios(idCedulaUsuario)
);

-- Crear la tabla Imagenes
CREATE TABLE Imagenes (
    idImagen INT PRIMARY KEY IDENTITY(1,1),
    urlImagen NVARCHAR(MAX) NOT NULL,
    fechaSubida DATETIME NOT NULL DEFAULT GETDATE()
);

-- Crear la tabla CarritoCompras
CREATE TABLE CarritoCompras (
    idCarrito INT PRIMARY KEY IDENTITY(1,1),
    idProducto INT NOT NULL,
    idCedulaUsuarioCompra INT NOT NULL,
	cantidadProducto INT NOT NULL,
    fechaSeleccion DATETIME NOT NULL,
    FOREIGN KEY (idProducto) REFERENCES Productos(idProducto),
    FOREIGN KEY (idCedulaUsuarioCompra) REFERENCES Usuarios(idCedulaUsuario)
);

-- Insertar datos en la tabla Categorias
INSERT INTO Categorias (descripcionCategoria) VALUES
('Electrónica'),
('Ropa'),
('Hogar'),
('Deportes');

-- Insertar datos en la tabla Permisos
INSERT INTO Permisos (descripcionPermiso) VALUES
('Administrador'),
('Cliente');

INSERT INTO Imagenes( urlImagen, fechaSubida) VALUES
('/imagenes/ea5bae86-a108-447b-9b25-474f53ee7072.jpg', GETDATE()),
('/imagenes/fbecc4ab-7c52-48db-a5ad-0b2f4768bf88.jpg', GETDATE()),
('/imagenes/7925017d-dd73-4dd4-abde-84da724c6d74.jpg', GETDATE());


-- Insertar datos en la tabla Usuarios
INSERT INTO Usuarios (idCedulaUsuario, nombreUsuario, apellidoUsuario, correoelectronicoUsuario, contrasenaUsuario, idPermiso, estado) VALUES
(1, 'Admin', 'Principal', 'admin@example.com', '$2a$11$TUGLdteExN9u2MuaHID98eXP5Vqv2ZxQqEAFxkIQT6q/kfrdyXSR2', 1, 1), -- Cuenta admin
(2, 'Cliente', 'Perez', 'juan.perez@example.com', '$2a$11$OxULYAo41cRjhJ6ZNvu3eefvft8z4eMtQzJYer.TXoW4Eh/QtZudi', 2, 1); -- Cuenta cliente 

-- Insertar datos en la tabla Productos
INSERT INTO Productos (descripcionProducto, detallesProducto, precioProducto, cantidadProducto, fechaPublicacion, idCategoria, idCedulaUsuarioRegistra, estado, idImagen) VALUES
('Camiseta Deportiva', 'Camiseta para entrenamiento, talla L', 25.50, 50, GETDATE(), 2, 1, 1, 1),
('Laptop HP', 'Laptop de alto rendimiento, 16GB RAM, SSD 512GB', 1200.00, 10, GETDATE(), 1, 2, 1, 2),
('Balon futbol', 'Balon para entrenamiento', 25.50, 50, GETDATE(), 2, 1, 1, 3);


