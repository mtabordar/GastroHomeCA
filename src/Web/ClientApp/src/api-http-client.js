/* eslint-disable */
/* tslint:disable */
/**
 * HTTP Client Singleton for API calls
 */

import { HttpClient } from './web-api-client';

const baseUrl = import.meta.env.VITE_API_BASE_URL || import.meta.env.VITE_APP_API_URL || 'http://localhost:5000';

export function createSingletonHttpClient() {
  let httpClient = null;

  return function getHttpClient() {
    if (!httpClient) {
      httpClient = new HttpClient(baseUrl);
    }
    return httpClient;
  };
}

export class HttpClient {
  constructor(baseUrl) {
    this.baseUrl = baseUrl;
  }

  get(url) {
    return fetch(`${this.baseUrl}${url}`).then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    });
  }

  post(url, body) {
    return fetch(`${this.baseUrl}${url}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      },
      body: JSON.stringify(body)
    }).then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    });
  }

  put(url, body) {
    return fetch(`${this.baseUrl}${url}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      },
      body: JSON.stringify(body)
    }).then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    });
  }

  delete(url) {
    return fetch(`${this.baseUrl}${url}`, {
      method: 'DELETE'
    }).then(response => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    });
  }
}
