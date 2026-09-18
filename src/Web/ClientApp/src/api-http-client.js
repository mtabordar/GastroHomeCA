/* eslint-disable */
/* tslint:disable */
/**
 * HTTP Client Singleton for API calls
 */

const baseUrl = import.meta.env.VITE_API_BASE_URL || import.meta.env.VITE_APP_API_URL || 'http://localhost:5000';

let httpClient = null;

export function createSingletonHttpClient() {
  if (!httpClient) {
    httpClient = new HttpClient(baseUrl);
  }
  return httpClient;
}

export default httpClient;

class HttpClient {
  constructor(baseUrl) {
    this.baseUrl = baseUrl;
  }

  get(endpoint) {
    return fetch(`${this.baseUrl}${endpoint}`)
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      });
  }

  post(endpoint, data) {
    return fetch(`${this.baseUrl}${endpoint}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      });
  }

  put(endpoint, data) {
    return fetch(`${this.baseUrl}${endpoint}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      });
  }

  delete(endpoint) {
    return fetch(`${this.baseUrl}${endpoint}`, {
      method: 'DELETE'
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      });
  }
}
