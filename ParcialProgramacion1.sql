-- ==============================================
-- PRUEBA PARCIAL: PROGRAMACIÓN I
-- Base de datos: ParcialProgramacion1
-- Sistema de control de productos
-- Autor: Guillermo Antonio Pérez
-- ==============================================

-- 1. Crear la base de datos
IF DB_ID('ParcialProgramacion1') IS NULL
BEGIN
    CREATE DATABASE ParcialProgramacion1;
END;
GO

USE ParcialProgramacion1;
GO

-- 2. Crear la tabla Productos
  CREATE TABLE dbo.Productos
    (
        IdProducto INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Categoria NVARCHAR(50) NOT NULL,
        Precio DECIMAL(10,2) NOT NULL,
        Existencias INT NOT NULL
    );
GO

-- 3. Registrar al menos tres productos de ejemplo
INSERT INTO dbo.Productos (Nombre, Categoria, Precio, Existencias)
VALUES
    (N'Oso de peluche grande', N'Osos', 29.99, 12),
    (N'Conejo de peluche', N'Animales', 22.50, 18),
    (N'Unicornio arcoíris', N'Fantasía', 27.00, 10),
    (N'Panda de peluche', N'Osos', 24.99, 15),
    (N'Elefante de peluche', N'Animales', 20.00, 20);
GO

SELECT * FROM dbo.Productos;
GO
