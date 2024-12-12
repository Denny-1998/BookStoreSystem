# Bookstore System

A bookstore application demonstrating the use of both relational (SQL Server) and NoSQL (Redis) databases.

## Database Design Choices

### SQL Server (Relational Database)
Used for storing core business data that requires:
- Data consistency
- Complex relationships
- Transaction support

#### Tables:
1. **Books**
   - Permanent storage of book information
   - Relationships with authors
   - Inventory tracking

2. **Authors**
   - Author information
   - Many-to-many relationship with books

3. **Customers**
   - Customer information
   - One-to-many relationship with orders

4. **Orders**
   - Order processing
   - Complex relationships (Customer, OrderItems)
   - Transaction support for inventory updates

### Redis (NoSQL)
Used for:
1. **Book Caching**
   - Frequently accessed book details are cached
   - Improves read performance for popular books

2. **Recent Orders List**
   - Maintains list of 10 most recent orders
   - Fast access to frequently needed order data

## Setup Instructions

### Prerequisites
- Docker
- Docker Compose

### Running the Application

1. Clone the repository

2. Navigate to project directory and run:
```bash
docker-compose up
```

This will start:
- ASP.NET Core application
- SQL Server database
- Redis cache

### Database Structure

```sql
Books
- Id (PK)
- Title
- ISBN
- Price
- StockQuantity

Authors
- Id (PK)
- Name

Customers
- Id (PK)
- Name
- Email

Orders
- Id (PK)
- CustomerId (FK)
- OrderDate
- TotalAmount

OrderItems
- Id (PK)
- OrderId (FK)
- BookId (FK)
- Quantity
- UnitPrice
```

## Key Features
- Automatic caching of book details in Redis
- Recent orders stored in Redis for quick access
- Inventory management with SQL transactions
- Basic CRUD operations for all entities
- Relationship management between entities

## Technical Implementation
- ASP.NET Core Web API
- Entity Framework Core for SQL Server
- StackExchange.Redis for Redis operations
- Docker containerization
