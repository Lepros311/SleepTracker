export interface PagedResponse<T> {
  status: number;
  message?: string;
  data?: T;
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}

