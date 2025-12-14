import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResponse } from '../models/paged-response';
import { SleepReadDto } from '../models/sleep-read-dto';

@Injectable({
  providedIn: 'root',
})
export class SleepService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getSleeps(pageNumber: number = 1, pageSize: number = 10): Observable<PagedResponse<SleepReadDto[]>> {
    const params = new HttpParams().set('pageNumber', pageNumber.toString()).set('pageSize', pageSize.toString());
    return this.http.get<PagedResponse<SleepReadDto[]>>(`${this.baseUrl}/sleeps`, { params });
  }
}
