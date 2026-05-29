export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface PageRequest {
  page?: number;
  pageSize?: number;
  search?: string;
}
