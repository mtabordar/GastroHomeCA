# GastroHome - Plan Funcional

## Visión General
Aplicación para gestionar productos, realizar listas de compras, hacer seguimiento de viajes de compra, monitorear historial de precios y recibir recomendaciones inteligentes.

---

## Estado Actual

### ✅ Implementados
- **Gestión de Productos:** Crear, editar y eliminar productos (nombre, categoría, código de barras, precio actual)
- **Listas de Compras:** Crear listas de compras con nombre, agregar productos, marcar como comprados, cantidad y notas
- **Listas de Tareas:** Crear listas de tareas con prioridades
- **Autenticación:** Registro, inicio y cierre de sesión de usuarios

### 🔲 Pendientes
- Historial de precios
- Viajes de compra
- Historial de compras
- Recomendaciones inteligentes

---

## 1. Gestión de Productos

### Funcionalidades Actuales
- Agregar productos con nombre, categoría, código de barras y precio
- Editar productos existentes
- Eliminar productos
- Ver lista de todos los productos
- Ver detalle de un producto

### Funcionalidades Pendientes
- **Historial de Precios:** Registrar y visualizar cómo ha cambiado el precio de cada producto a lo largo del tiempo
- **Código de Barras:** Buscar productos existentes por código de barras para evitar duplicados
- **Gestión de Categorías:** Organizar productos por categorías predefinidas en lugar de texto libre
- **Control de Inventario:** Registrar cantidad disponible de cada producto

---

## 2. Viajes de Compra

### Descripción
Registrar cada visita a la tienda para llevar un control de qué se compró, cuándo y cuánto se gastó.

### Requisitos Funcionales
- Crear viajes de compra con fecha y opciónal nota
- Agregar productos al viaje de compra
- Especificar cantidad comprada de cada producto
- Especificar precio pagado (puede diferir del precio registrado)
- Calcular total gastado por viaje
- Ver lista de viajes pasados con resumen de gastos
- Ver detalle de un viaje con todos los productos comprados

---

## 3. Historial de Compras

### Descripción
Consultar y analizar todas las compras realizadas a lo largo del tiempo.

### Requisitos Funcionales
- Ver historial completo de compras
- Filtrar compras por rango de fechas
- Filtrar compras por producto
- Filtrar compras por viaje de compra
- Mostrar resumen con:
  - Productos más comprados
  - Total gastado por período
  - Frecuencia de compras por producto
- Identificar patrones de compra

---

## 4. Recomendaciones Inteligentes

### Descripción
Sugerir productos para comprar basándose en el historial y patrones del usuario.

### Requisitos Funcionales
- Sugerir productos por frecuencia de compra anterior
- Alertar cuando un producto no se ha comprado en más tiempo del habitual
- Sugerir productos basándose en tendencias de cantidad
- Notificar cuando el precio de un producto habitual ha bajado
- Lista de "próximamente se acabará" basada en consumo

---

## 5. Mejoras a Funcionalidades Existentes

### Listas de Compras ✅
- Crear listas de compras con nombre
- Agregar productos a la lista de compra
- Marcar productos como comprados
- Lista de compras persistente entre viajes
- Cantidad y notas por producto

### Autenticación
- Perfiles de usuario con preferencias
- Configuración de moneda y unidad de medida

---

## Prioridad de Desarrollo

| Prioridad | Feature | Justificación |
|-----------|---------|---------------|
| 1 | Listas de Compras | Base para viajes de compra |
| 2 | Viajes de Compra | Funcionalidad central de la app |
| 3 | Historial de Compras | Necesario para recomendaciones |
| 4 | Historial de Precios | Complementa historial de compras |
| 5 | Recomendaciones Inteligentes | Depende de historiales previos |
| 6 | Gestión de Categorías | Mejora organización de productos |
| 7 | Control de Inventario | Útil pero no crítico |
| 8 | Búsqueda por Código de Barras | Mejora UX pero no esencial |

---

*Última Actualización: Sesión Actual*
