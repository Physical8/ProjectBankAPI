# 📌 API Bancaria en .NET 8

🚀 **API para gestión bancaria** desarrollada con **ASP.NET Core 8** y **SQL Server**, que permite realizar operaciones sobre **clientes, cuentas bancarias y transacciones** con lógica de negocio integrada.

---

## 📋 **Características**
✅ CRUD completo para **Clientes, Cuentas y Transacciones**.  
✅ Aplicación de **reglas de negocio** para retiros, consignaciones y transferencias.  
✅ **Autenticación JWT** (en caso de que la agregues después).  
✅ **Patrones de diseño:** CQRS, Mediator y Repository.  
✅ **Logs de auditoría** con **Serilog**.  
✅ **Pruebas unitarias** con **xUnit, Moq y FluentAssertions**.  
✅ **Soporte para pruebas funcionales con Postman**.  

---

## 🛠 **Tecnologías Utilizadas**
- **.NET 8**
- **Entity Framework Core**
- **SQL Server**
- **MediatR**
- **Serilog**
- **xUnit, Moq y FluentAssertions** (para pruebas)
- **Swagger** (Documentación de la API)
- **Postman** (Pruebas funcionales)

📌 Endpoints Disponibles
🔹 Clientes
Método	Endpoint	Descripción
GET	/api/clients	Obtener todos los clientes
GET	/api/clients/{id}	Obtener un cliente por ID
POST	/api/clients	Crear un nuevo cliente
PUT	/api/clients/{id}	Actualizar un cliente
DELETE	/api/clients/{id}	Eliminar un cliente
🔹 Cuentas Bancarias
Método	Endpoint	Descripción
GET	/api/bankaccounts	Obtener todas las cuentas
GET	/api/bankaccounts/{id}	Obtener una cuenta por ID
POST	/api/bankaccounts	Crear una cuenta bancaria
PUT	/api/bankaccounts/{id}	Actualizar una cuenta bancaria
DELETE	/api/bankaccounts/{id}	Eliminar una cuenta bancaria
🔹 Transacciones
Método	Endpoint	Descripción
GET	/api/transactions	Obtener todas las transacciones
GET	/api/transactions/{id}	Obtener una transacción por ID
POST	/api/transactions	Crear una transacción (retiro, consignación o transferencia)
