import { HttpParams, HttpResponse } from "@angular/common/http";
import { signal } from "@angular/core";
import { PaginatedResult } from "../_models/pagination";

export function setPaginatedResponse<T>(response: HttpResponse<T>,
  paginatedResultSignal: ReturnType<typeof signal<PaginatedResult<T> | null>>) {
  const paginationHeader = response.headers.get('Pagination');
  let pagination = null;
  if (paginationHeader) {
      try {
          pagination = JSON.parse(paginationHeader);
      } catch (error) {
          console.error('Pagination header parse error:', error);
      }
  }
  paginatedResultSignal.set({
      items: response.body as T,
      pagination: pagination
  });
}


export function setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    if (pageNumber && pageSize) {
        params = params.append('pageNumber', pageNumber);
        params = params.append('pageSize', pageSize);
    }

    return params;
}
