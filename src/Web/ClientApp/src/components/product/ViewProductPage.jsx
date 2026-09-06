import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ProductsClient, ProductDto } from '../../web-api-client';
import { createSingletonHttpClient } from '../../api-http-client';

const httpClient = createSingletonHttpClient();
const productsClient = new ProductsClient(httpClient);

export function ViewProductPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
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
  }, [id]);

  const handleDelete = async () => {
    if (!confirm('Are you sure you want to delete this product?')) {
      return;
    }

    try {
      // Note: We don't have a delete endpoint yet, so we'll just navigate back
      // You can add a DELETE endpoint later
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
        <button style={styles.backButton} onClick={handleBack}>
          Back to Products
        </button>
      </div>
    );
  }

  if (!product) {
    return (
      <div style={styles.container}>
        <h1 style={styles.title}>Product Not Found</h1>
        <button style={styles.backButton} onClick={handleBack}>
          Back to Products
        </button>
      </div>
    );
  }

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <h1 style={styles.title}>Product Details</h1>
        <div style={styles.buttonGroup}>
          <button style={styles.backButton} onClick={handleBack}>
            &larr; Back
          </button>
          <button style={styles.deleteButton} onClick={handleDelete}>
            Delete
          </button>
        </div>
      </div>

      <div style={styles.card}>
        <div style={styles.field}>
          <span style={styles.label}>Product Name:</span>
          <span style={styles.value}>{product.Name}</span>
        </div>

        <div style={styles.field}>
          <span style={styles.label}>Category:</span>
          <span style={styles.value}>{product.Category}</span>
        </div>

        {product.Barcode && (
          <div style={styles.field}>
            <span style={styles.label}>Barcode:</span>
            <span style={styles.value}>{product.Barcode}</span>
          </div>
        )}

        <div style={styles.field}>
          <span style={styles.label}>Current Price:</span>
          <span style={styles.price}>{product.CurrentPrice.toFixed(2)}</span>
        </div>

        <div style={styles.field}>
          <span style={styles.label}>ID:</span>
          <span style={styles.value}>{product.Id}</span>
        </div>
      </div>
    </div>
  );
}

const styles = {
  container: {
    maxWidth: '700px',
    margin: '0 auto',
    padding: '20px'
  },
  title: {
    textAlign: 'center',
    marginBottom: '20px',
    color: '#333'
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '30px'
  },
  buttonGroup: {
    display: 'flex',
    gap: '10px'
  },
  card: {
    border: '1px solid #ddd',
    borderRadius: '8px',
    padding: '25px',
    backgroundColor: '#f9f9f9'
  },
  field: {
    marginBottom: '15px',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start'
  },
  label: {
    fontWeight: '600',
    fontSize: '14px',
    color: '#555',
    minWidth: '120px'
  },
  value: {
    fontSize: '16px',
    color: '#333'
  },
  price: {
    fontSize: '18px',
    color: '#007bff',
    fontWeight: '600'
  },
  backButton: {
    padding: '10px 20px',
    fontSize: '14px',
    backgroundColor: '#6c757d',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    transition: 'background-color 0.2s'
  },
  deleteButton: {
    padding: '10px 20px',
    fontSize: '14px',
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
    borderRadius: '4px',
    marginBottom: '20px',
    textAlign: 'center'
  }
};