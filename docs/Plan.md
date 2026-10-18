# GastroHome - Frontend Documentation

## Overview
A food handler application to manage products, track shopping trips, monitor price history, and receive intelligent recommendations.

---

## 1. Core Features & Requirements

### A. Product Management
- Add/Edit/Delete products (name, category, barcode, etc.)
- Track price history for each product
- Automatically use the latest price unless manually updated

### B. Shopping Trip Tracking
- Record purchases during each store visit:
  - Select products from existing list
  - Optionally specify quantity and unit price
- Calculate total spending per trip

### C. Purchase History & Analytics
- View purchase history filtered by date range or product
- Display summaries:  
  - Most purchased items  
  - Total spent over time  
  - Frequency of purchases

### D. Smart Recommendations
- Suggest products to repurchase based on:
  - Past buying frequency  
  - Quantity trends (e.g., running low)  
  - Seasonal or habitual patterns

---

## 2. Domain Entities & Relationships

### Entities:

1. **Product**
   - `Id` (PK, int/UUID)
   - `Name` (string, max length)
   - `Category` (string, enum or string)
   - `Barcode` (string, optional but recommended for uniqueness)
   - `CurrentPrice` (decimal, nullable or non-nullable depending on design)
   - `LastUpdatedDate` (datetime)
   - Relationship to `PriceHistory`

2. **PriceHistory**
   - `Id` (PK, int/UUID)
   - `ProductId` (FK → Product)
   - `OldPrice` (decimal, nullable or not depending on first entry)
   - `NewPrice` (decimal)
   - `UpdateDate` (datetime)
   - Reason for update (optional: "stock change", "promotion", etc.)

3. **ShoppingTrip**
   - `Id` (PK, int/UUID)
   - `Date` (datetime, nullable or not depending on design)
   - `TotalCost` (decimal, calculated sum of all items in this trip)
   - Relationship to `PurchasedItem`

4. **PurchasedItem** (composite entity or junction table)
   - `ShoppingTripId` (FK → ShoppingTrip)
   - `ProductId` (FK → Product)
   - `Quantity` (int, must be > 0)
   - `UnitPrice` (decimal, snapshot at time of purchase)

---

## 3. API Design (RESTful Endpoints)

### Products
| Method | Endpoint             | Description                          |
|--------|----------------------|-------------------------------------|
| POST   | `/api/products`      | Add new product                     |
| GET    | `/api/products/{id}` | Get product with current price & history count |
| PUT    | `/api/products/{id}` | Update price or metadata            |

### Shopping Trips
| Method | Endpoint                 | Description                                |
|--------|--------------------------|-------------------------------------------|
| POST   | `/api/shoppingtrips`     | Record a new shopping trip with items     |
| GET    | `/api/shoppingtrips`     | List all trips (with filters)             |

### Analytics & Recommendations
| Method | Endpoint                       | Description                                |
|--------|--------------------------------|-------------------------------------------|
| GET    | `/api/recommendations/top-items` | Products purchased frequently            |
| GET    | `/api/analytics/spending-over-time`  | Total cost broken down by date range     |

---

## 4. Service Layer Responsibilities

### Services:
1. **ProductService**
   - CRUD operations for products
   - Manage price history updates (auto-save new price on edit)

2. **TripService**
   - Validate trip data (quantity > 0, product exists)
   - Calculate total cost per trip
   - Store trip records

3. **RecommendationService**
   - Aggregate purchase frequency
   - Suggest top items to restock

4. **AnalyticsService**
   - Generate spending trends
   - Filter and format historical data

---

## 5. Data Flow Example
1. **User adds a product**:  
   - Create `Product` record with initial price
2. **First store visit**:  
   - User selects product → create `PurchasedItem` + optional `PriceHistory` entry
3. **Price increases**:  
   - Update `Product.CurrentPrice`, insert new row in `PriceHistory`
4. **Next visit**:  
   - System uses latest price automatically unless overridden

---

## 6. Technical Considerations
- **Database Schema**: Normalized relational model (SQLite)
- **Validation**: Ensure quantities and prices are valid before saving
- **Performance**: Index foreign keys (`ProductId` in `PurchasedItem`)
- **Security**: Mask sensitive info if needed (e.g., payment methods not stored here)

---

## 7. Future Enhancements
- User authentication & authorization
- Push notifications for low stock recommendations
- Export purchase history to CSV/Excel
- Visual analytics dashboard' > ../docs/PROJECT_REQUIREMENTS.md