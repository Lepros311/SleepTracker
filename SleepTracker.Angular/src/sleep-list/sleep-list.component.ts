import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SleepService } from '../app/services/sleep';
import { SleepReadDto } from '../app/models/sleep-read-dto';
import { PagedResponse } from '../app/models/paged-response';

@Component({
  selector: 'app-sleep-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sleep-list.component.html',
  styleUrl: './sleep-list.component.css'
})
export class SleepListComponent implements OnInit {
  sleeps = signal<SleepReadDto[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  pageNumber = signal(1);
  pageSize = signal(10);
  totalRecords = signal(0);
  totalPages = signal(0);

  constructor(private sleepService: SleepService) {}

  ngOnInit(): void { this.loadSleeps(); }

  loadSleeps(): void {
    this.loading.set(true);
    this.error.set(null);

    this.sleepService.getSleeps(this.pageNumber(), this.pageSize()).subscribe({
      next: (response: PagedResponse<SleepReadDto[]>) => {
        // Debug: log the response to see its structure
        console.log('Full response:', response);
        console.log('Status value:', response.status);
        console.log('Status type:', typeof response.status);
        if (response.status === 0 && response.data) {
          this.sleeps.set(response.data);
          this.totalRecords.set(response.totalRecords);
          this.totalPages.set(response.totalPages);
        } else {
          this.error.set(response.message || 'Failed to load sleep records');
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load sleep records. Please try again.');
        this.loading.set(false);
        console.error('Error loading sleeps:', err);
      }
    });
  }

  formatDateTime(dateTimeString: string): string {
    const date = new Date(dateTimeString);
    return date.toLocaleString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: 'numeric',
      minute: '2-digit',
      hour12: true
    });
  }
}