import React, { useState, useEffect } from 'react';
import { ProductsClient, ProductDto } from '../../web-api-client';
import { createSingletonHttpClient } from '../../api-http-client';
import { useNavigate } from 'react-router-dom';

const httpClient = createSingletonHttpClient();
const productsClient = new ProductsClient(null, httpClient);

export function ProductListPage() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        const result = await productsClient.productsAll();
        setProducts(result || []);
        setError('');
      } catch (err) {
        setError(err.message || 'Failed to load products');
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);
  const handleViewProduct = (productId) => {
    navigate(`/product/${productId}`);
  };

  const handleCreateNew = () => {
    navigate('/product');
  };

  if (loading) {
    return (
      <div style={styles.container}>
        <h1 style={styles.title}>Loading Products...</h1>
      </div>
    );
  }

  if (error) {
    return (
      <div style={styles.container}>
        <h1 style={styles.title}>Error</h1>
        <div style={styles.errorMessage}>{error}</div>
      </div>
    );
  }

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <h1 style={styles.title}>Product Management</h1>
        <button style={styles.createNewButton} onClick={handleCreateNew}>
          + Create New Product
        </button>
      </div>

      {products.length === 0 ? (
        <div style={styles.noProducts}>
          <p>No products found. Create your first product!</p>
        </div>
      ) : (
        <div style={styles.grid}>
          {products.map((product) => (
            <div key={product.Id} style={styles.card}>
              <div style={styles.cardHeader}>
                <h3 style={styles.cardTitle}>{product.Name}</h3>
                <span style={styles.price}>{product.CurrentPrice.toFixed(2)}</span>
              </div>
              <div style={styles.cardBody}>
                <p><strong>Category:</strong> {product.Category}</p>
                {product.Barcode && (
                  <p><strong>Barcode:</strong> {product.Barcode}</p>
                )}
                <p><strong>ID:</strong> {product.Id}</p>
              </div>
              <div style={styles.cardFooter}>
                <button
                  style={styles.viewButton}
                  onClick={() => handleViewProduct(product.Id)}
                >
                  View Details
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

const styles = {
  container: {
    maxWidth: '1200px',
    margin: '0 auto',
    padding: '20px'
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '30px'
  },
  title: {
    margin: 0,
    color: '#333'
  },
  createNewButton: {
    padding: '12px 24px',
    fontSize: '16px',
    backgroundColor: '#28a745',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    transition: 'background-color 0.2s'
  },
  noProducts: {
    textAlign: 'center',
    padding: '60px 20px',
    backgroundColor: '#f9f9f9',
    borderRadius: '8px'
  },
  grid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
    gap: '20px'
  },
  card: {
    border: '1px solid #ddd',
    borderRadius: '8px',
    padding: '20px',
    backgroundColor: '#fff',
    transition: 'box-shadow 0.2s',
    cursor: 'pointer'
  },
  cardHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '15px'
  },
  cardTitle: {
    margin: 0,
    fontSize: '18px',
    color: '#333'
  },
  price: {
    fontSize: '20px',
    fontWeight: '600',
    color: '#007bff'
  },
  cardBody: {
    fontSize: '14px',
    color: '#666'
  },
  cardFooter: {
    marginTop: '15px',
    textAlign: 'right'
  },
  viewButton: {
    padding: '10px 20px',
    fontSize: '14px',
    backgroundColor: '#007bff',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    transition: 'background-color 0.2s'
  },
  errorMessage: {
    padding: '15px',
    backgroundColor: '#f8d7da',
    color: '#721c24',
    borderRadius: '4px'
  }
};

export default ProductListPage;

