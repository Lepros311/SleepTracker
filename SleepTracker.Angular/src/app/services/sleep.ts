import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SleepService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getSleeps() {
    return this.http.get(`${this.baseUrl}/sleeps`);
  }
}
