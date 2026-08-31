/* eslint-disable */
/* tslint:disable */
/**
 * Generated using TypeScript Code Generator
 *
 * @see https://github.com/RicoSuter/NSwag
 *
 * @overview
 * This file was automatically generated. Any changes will be overwritten.
 * To regenerate this file, run `npm run generate-api`.
 */

export class SwaggerException extends Error {
  public readonly statusCode: number;
  public readonly responseBody: string;

  constructor(statusCode: number, responseBody: string) {
    super();
    this.statusCode = statusCode;
    this.responseBody = responseBody;
  }
}

export const ProductsClient = class {
  private readonly _httpClient: HttpClient;

  constructor(httpClient: HttpClient) {
    this._httpClient = httpClient;
  }

  /**
   * Create a new product.
   */
  public async createProduct(
    body: CreateProductRequest
  ): Promise<void> {
    return this._httpClient
      .post('/api/Products', body)
      .then((response) => {
        if (response.status >= 400) {
          throw new SwaggerException(response.status, response.text());
        }
      });
  }
};

export class CreateProductRequest {
  public name: string;
  public category: string;
  public barcode: string | undefined;
  public currentPrice: number;

  constructor(name: string, category: string, barcode: string | undefined, currentPrice: number) {
    this.name = name;
    this.category = category;
    this.barcode = barcode;
    this.currentPrice = currentPrice;
  }
};

export interface HttpClient {
  get(url: string): Promise<Response>;
  post(url: string, body: any): Promise<Response>;
  put(url: string, body: any): Promise<Response>;
  delete(url: string): Promise<Response>;
}
