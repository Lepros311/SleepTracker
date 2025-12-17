import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResponse } from '../models/paged-response';
import { SleepReadDto } from '../models/sleep-read-dto';
import { SleepUpdateDto } from '../models/sleep-update-dto';
import { SleepCreateDto } from '../models/sleep-create-dto';

@Injectable({
  providedIn: 'root',
})
export class SleepService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getSleeps(pageNumber: number = 1, pageSize: number = 10, startDate?: Date, endDate?: Date): Observable<PagedResponse<SleepReadDto[]>> {
    let params = new HttpParams().set('pageNumber', pageNumber.toString()).set('pageSize', pageSize.toString());

    if (startDate) { params = params.set('Start', startDate.toISOString()); }

    if (endDate) { params = params.set('End', endDate.toISOString()); }

    return this.http.get<PagedResponse<SleepReadDto[]>>(`${this.baseUrl}/sleeps`, { params });
  }

  createSleep(createDto: SleepCreateDto): Observable<SleepReadDto> {
    return this.http.post<SleepReadDto>(`${this.baseUrl}/sleeps`, createDto);
  }

  updateSleep(id: number, updateDto: SleepUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/sleeps/${id}`, updateDto);
  }

  deleteSleep(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/sleeps/${id}`);
  }
}
