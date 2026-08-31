import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { ProductsClient, CreateProductRequest, HttpClient } from '../web-api-client';
import { createSingletonHttpClient } from '../api-http-client';

const httpClient = createSingletonHttpClient();
const productsClient = new ProductsClient(httpClient);

export function CreateProductPage() {
  const [formData, setFormData] = useState({
    Name: '',
    Category: '',
    Barcode: '',
    CurrentPrice: 0
  });

  const [errors, setErrors] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();
  const location = useLocation();

  const requiredFields = ['Name', 'Category'];

  const validateForm = () => {
    const newErrors = {};

    requiredFields.forEach(field => {
      if (!formData[field] || formData[field].trim() === '') {
        newErrors[field] = `${field} is required`;
      }
    });

    if (formData.CurrentPrice < 0) {
      newErrors.CurrentPrice = 'Price cannot be negative';
    }

    const barcodePattern = /^[0-9\s\-]*$/;
    if (formData.Barcode && !barcodePattern.test(formData.Barcode)) {
      newErrors.Barcode = 'Barcode must contain only numbers, spaces, and dashes';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);

    try {
      await productsClient.createProduct({
        name: formData.Name,
        category: formData.Category,
        barcode: formData.Barcode || undefined,
        currentPrice: formData.CurrentPrice
      });

      setSuccessMessage('Product created successfully!');
      setErrorMessage('');

      setTimeout(() => {
        navigate(-1);
      }, 1500);
    } catch (error) {
      setSuccessMessage('');
      setErrorMessage(error.message || 'Failed to create product');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));

    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  return (
    <div style={styles.container}>
      <h1 style={styles.title}>Create Product</h1>

      {successMessage && (
        <div style={styles.successMessage}>{successMessage}</div>
      )}

      {errorMessage && (
        <div style={styles.errorMessage}>{errorMessage}</div>
      )}

      <form onSubmit={handleSubmit} style={styles.form}>
        <div style={styles.formGroup}>
          <label style={styles.label} htmlFor="Name">
            Product Name *
          </label>
          <input
            style={styles.input}
            type="text"
            id="Name"
            name="Name"
            value={formData.Name}
            onChange={handleChange}
            placeholder="Enter product name"
            disabled={isSubmitting}
          />
          {errors.Name && <span style={styles.errorText}>{errors.Name}</span>}
        </div>

        <div style={styles.formGroup}>
          <label style={styles.label} htmlFor="Category">
            Category *
          </label>
          <input
            style={styles.input}
            type="text"
            id="Category"
            name="Category"
            value={formData.Category}
            onChange={handleChange}
            placeholder="Enter product category"
            disabled={isSubmitting}
          />
          {errors.Category && <span style={styles.errorText}>{errors.Category}</span>}
        </div>

        <div style={styles.formGroup}>
          <label style={styles.label} htmlFor="Barcode">
            Barcode
          </label>
          <input
            style={styles.input}
            type="text"
            id="Barcode"
            name="Barcode"
            value={formData.Barcode}
            onChange={handleChange}
            placeholder="Enter barcode (optional, numbers, spaces, and dashes only)"
            disabled={isSubmitting}
          />
          {errors.Barcode && <span style={styles.errorText}>{errors.Barcode}</span>}
        </div>

        <div style={styles.formGroup}>
          <label style={styles.label} htmlFor="CurrentPrice">
            Current Price ($) *
          </label>
          <input
            style={styles.input}
            type="number"
            id="CurrentPrice"
            name="CurrentPrice"
            value={formData.CurrentPrice}
            onChange={handleChange}
            placeholder="0.00"
            step="0.01"
            min="0"
            disabled={isSubmitting}
          />
          {errors.CurrentPrice && <span style={styles.errorText}>{errors.CurrentPrice}</span>}
        </div>

        <div style={styles.formActions}>
          <button
            type="button"
            style={styles.cancelButton}
            onClick={() => navigate(-1)}
            disabled={isSubmitting}
          >
            Cancel
          </button>
          <button
            type="submit"
            style={styles.submitButton}
            disabled={isSubmitting}
          >
            {isSubmitting ? 'Creating...' : 'Create Product'}
          </button>
        </div>
      </form>
    </div>
  );
}

const styles = {
  container: {
    maxWidth: '600px',
    margin: '0 auto',
    padding: '20px'
  },
  title: {
    textAlign: 'center',
    marginBottom: '30px',
    color: '#333'
  },
  successMessage: {
    padding: '15px',
    backgroundColor: '#d4edda',
    color: '#155724',
    borderRadius: '4px',
    marginBottom: '20px',
    textAlign: 'center'
  },
  errorMessage: {
    padding: '15px',
    backgroundColor: '#f8d7da',
    color: '#721c24',
    borderRadius: '4px',
    marginBottom: '20px',
    textAlign: 'center'
  },
  form: {
    display: 'flex',
    flexDirection: 'column',
    gap: '20px'
  },
  formGroup: {
    display: 'flex',
    flexDirection: 'column',
    gap: '8px'
  },
  label: {
    fontWeight: '600',
    fontSize: '14px',
    color: '#333'
  },
  input: {
    padding: '12px',
    fontSize: '16px',
    border: '1px solid #ddd',
    borderRadius: '4px',
    transition: 'border-color 0.2s'
  },
  errorText: {
    color: '#dc3545',
    fontSize: '12px'
  },
  formActions: {
    display: 'flex',
    gap: '10px',
    justifyContent: 'flex-end',
    marginTop: '10px'
  },
  cancelButton: {
    padding: '12px 24px',
    fontSize: '16px',
    backgroundColor: '#6c757d',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    transition: 'background-color 0.2s'
  },
  submitButton: {
    padding: '12px 24px',
    fontSize: '16px',
    backgroundColor: '#007bff',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    transition: 'background-color 0.2s'
  }
};