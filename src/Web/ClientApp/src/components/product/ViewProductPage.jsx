import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ProductsClient, ProductDto } from '../../web-api-client';
import { createSingletonHttpClient } from '../../api-http-client';

const httpClient = createSingletonHttpClient();

export function ViewProductPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [productsClient, setProductsClient] = useState(null);

  useEffect(() => {
    setProductsClient(new ProductsClient(null, httpClient()));
    
    const fetchProduct = async () => {
      try {
        setLoading(true);
        const productDto = await productsClient.products(parseInt(id));
        setProduct(productDto);
        setError('');
      } catch (err) {
        setError(err.message || 'Failed to load product');
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [id, productsClient]);

  const handleDelete = async () => {
    if (!confirm('Are you sure you want to delete this product?')) {
      return;
    }

    try {
      navigate('/product');
    } catch (err) {
      setError('Failed to delete product');
    }
  };

  const handleBack = () => {
    navigate('/product');
  };

  if (loading) {
    return (
      <div style={styles.container}>
        <h1 style={styles.title}>Loading Product...</h1>
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

  if (!product) {
    return (
      <div style={styles.container}>
        <h1 style={styles.title}>Product not found</h1>
      </div>
    );
  }

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <button style={styles.backButton} onClick={handleBack}>
          ← Back to Products
        </button>
        <h1 style={styles.title}>{product.Name}</h1>
      </div>

      <div style={styles.card}>
        <div style={styles.cardBody}>
          <p><strong>ID:</strong> {product.Id}</p>
          <p><strong>Name:</strong> {product.Name}</p>
          <p><strong>Category:</strong> {product.Category}</p>
          {product.Barcode && (
            <p><strong>Barcode:</strong> {product.Barcode}</p>
          )}
          <p><strong>Current Price:</strong> ${product.CurrentPrice.toFixed(2)}</p>
          <p><strong>Original Price:</strong> ${product.OriginalPrice?.toFixed(2) || 'N/A'}</p>
        </div>

        <div style={styles.cardFooter}>
          <button
            style={styles.editButton}
            onClick={() => handleBack()}
          >
            Edit Product
          </button>
          <button
            style={styles.deleteButton}
            onClick={handleDelete}
          >
            Delete Product
          </button>
        </div>
      </div>
    </div>
  );
}

const styles = {
  container: {
    maxWidth: '800px',
    margin: '0 auto',
    padding: '20px'
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '30px'
  },
  backButton: {
    padding: '10px 20px',
    fontSize: '16px',
    backgroundColor: '#6c757d',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    transition: 'background-color 0.2s'
  },
  title: {
    margin: 0,
    fontSize: '28px',
    color: '#333'
  },
  card: {
    border: '1px solid #ddd',
    borderRadius: '8px',
    padding: '25px',
    backgroundColor: '#fff',
    boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
  },
  cardBody: {
    fontSize: '16px',
    color: '#555',
    lineHeight: '1.8'
  },
  cardFooter: {
    display: 'flex',
    gap: '10px',
    marginTop: '30px',
    justifyContent: 'flex-end'
  },
  editButton: {
    padding: '12px 24px',
    fontSize: '16px',
    backgroundColor: '#007bff',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    transition: 'background-color 0.2s'
  },
  deleteButton: {
    padding: '12px 24px',
    fontSize: '16px',
    backgroundColor: '#dc3545',
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

export default ViewProductPage;
