import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { SleepService } from '../app/services/sleep';
import { SleepReadDto } from '../app/models/sleep-read-dto';
import { PagedResponse } from '../app/models/paged-response';
import { SleepEditDialogComponent } from './sleep-edit-dialog.component';
import { SleepDeleteDialogComponent } from './sleep-delete-dialog.component';
import { SleepCreateDialogComponent } from './sleep-create-dialog.component';

@Component({
  selector: 'app-sleep-list',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatDialogModule],
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

  timerRunning = signal(false);
  timerStartTime = signal<Date | null>(null);
  elapsedTime = signal<string>('00:00:00');
  private timerInterval: any = null;

  constructor(private sleepService: SleepService, private dialog: MatDialog) {}

  ngOnInit(): void { this.loadSleeps(); }

  ngOnDestroy(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }

  loadSleeps(): void {
    this.loading.set(true);
    this.error.set(null);

    this.sleepService.getSleeps(this.pageNumber(), this.pageSize()).subscribe({
      next: (response: PagedResponse<SleepReadDto[]>) => {
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

  startSleepTimer(): void {
    const startTime = new Date();
    this.timerStartTime.set(startTime);
    this.timerRunning.set(true);
    this.elapsedTime.set('00:00:00');
    
    this.timerInterval = setInterval(() => {
      if (this.timerStartTime()) {
        const now = new Date();
        const elapsed = Math.floor((now.getTime() - this.timerStartTime()!.getTime()) / 1000);
        this.elapsedTime.set(this.formatElapsedTime(elapsed));
      }
    }, 1000);
  }

  stopSleepTimer(): void {
    if (!this.timerStartTime()) {
      return;
    }

    // Clear the interval
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }

    const startTime = this.timerStartTime()!;
    const endTime = new Date();

    const createDto = {
      start: startTime.toISOString(),
      end: endTime.toISOString()
    };

    this.sleepService.createSleep(createDto).subscribe({
      next: () => {
        this.timerRunning.set(false);
        this.timerStartTime.set(null);
        this.loadSleeps();
      },
      error: (err) => {
        alert('Failed to create sleep record. Please try again.');
        console.error('Error creating sleep:', err);
      }
    });
  }

  formatElapsedTime(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(SleepCreateDialogComponent, {
      width: '90%',
      maxWidth: '525px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'created') {
        this.loadSleeps();
      }
    });
  }

  openEditDialog(sleep: SleepReadDto): void {
    const dialogRef = this.dialog.open(SleepEditDialogComponent, {
      width: '90%',
      maxWidth: '525px',
      data: { sleep }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'updated') {
        this.loadSleeps();
      }
    });
  }

  deleteSleep(sleep: SleepReadDto): void {
    const dialogRef = this.dialog.open(SleepDeleteDialogComponent, {
      width: '90%',
      maxWidth: '400px',
      data: {
        startDate: sleep.start,
        endDate: sleep.end,
        durationHours: sleep.durationHours
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.sleepService.deleteSleep(sleep.id).subscribe({
          next: () => {
            this.loadSleeps();
          },
          error: (err) => {
            alert('Failed to delete sleep record. Please try again.');
            console.error('Error deleting sleep:', err);
          }
        });
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