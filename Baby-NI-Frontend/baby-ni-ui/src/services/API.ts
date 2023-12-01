import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";

// Extend AxiosResponse interface to allow it to be treated as a Promise
declare module "axios" {
  interface AxiosResponse<T = any> extends Promise<T> {}
}

// Function to handle modifying the request object before sending
const handleRequest = (request: any) => {
  request.headers["Content-Type"] = "application/json";
  return request;
};

// Function to handle errors in  requests
const handleError = (error: any) => {
  // Check if the error response status is 401 (Unauthorized)
  if (401 === error.response?.status) {
    console.info("Session has expired!", error);
    window.location.replace("/Error/sessionExpired"); // Redirect to the session expired page
  }
  return Promise.reject(error);
};

// Abstract base class for API interactions
abstract class ApiBase {
  protected readonly instance: AxiosInstance;

  public constructor(baseURL: string) {
    this.instance = axios.create({
      baseURL,
    });

    this._initializeRequestInterceptor(); 
    this._initializeResponseInterceptor(); 
  }

  // Initialize the request interceptor to modify requests before sending
  private _initializeRequestInterceptor = () => {
    this.instance.interceptors.request.use(
      this._handleRequest,
      this._handleError 
    );
  };

  // Initialize the response interceptor to handle responses
  private _initializeResponseInterceptor = () => {
    this.instance.interceptors.response.use(
      this._handleResponse,
      this._handleError 
    );
  };

  // Function to handle the response data
  private _handleResponse = ({ data }: AxiosResponse) => {
    return data;
  };

  // Function to handle modifying the request config
  private _handleRequest = (config: AxiosRequestConfig) => {
    return handleRequest(config);
  };

  // Function to handle API errors
  protected _handleError = (error: any) => {
    return handleError(error);
  };
}

export { ApiBase };