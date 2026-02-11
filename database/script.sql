CREATE DATABASE MarketControl;
GO

USE MarketControl;
GO

CREATE TABLE Produto (
    Id INT IDENTITY PRIMARY KEY,
    Nome VARCHAR(100),
    Preco DECIMAL(10,2),
    Estoque INT
);

CREATE TABLE Cliente (
    Id INT IDENTITY PRIMARY KEY,
    Nome VARCHAR(100),
    Email VARCHAR(100)
);

CREATE TABLE Venda (
    Id INT IDENTITY PRIMARY KEY,
    ClienteId INT,
    Data DATETIME DEFAULT GETDATE(),
    Total DECIMAL(10,2),
    FOREIGN KEY (ClienteId) REFERENCES Cliente(Id)
);

CREATE TABLE VendaItem (
    Id INT IDENTITY PRIMARY KEY,
    VendaId INT,
    ProdutoId INT,
    Quantidade INT,
    PrecoUnitario DECIMAL(10,2),
    FOREIGN KEY (VendaId) REFERENCES Venda(Id),
    FOREIGN KEY (ProdutoId) REFERENCES Produto(Id)
);
