import { CreateProductPage } from "./components/product";
import { ViewProductPage } from "./components/product";
import { ProductListPage } from "./components/product";
import { EditProductPage } from "./components/product";
import { LoginPage } from "./components/api-authorization/LoginPage";
import { RegisterPage } from "./components/api-authorization/RegisterPage";
import { ProtectedRoute } from "./components/api-authorization/ProtectedRoute";

const AppRoutes = [
  {
    path: '/products',
    element: <ProductListPage />
  },
  {
    path: '/product',
    element: <CreateProductPage />
  },
  {
    path: '/product/create',
    element: <CreateProductPage />
  },
  {
    path: '/product/:id',
    element: <ViewProductPage />
  },
  {
    path: '/product/:id/edit',
    element: <EditProductPage />
  },
  {
    path: '/login',
    element: <LoginPage />
  },
  {
    path: '/register',
    element: <RegisterPage />
  }
];

export default AppRoutes;
