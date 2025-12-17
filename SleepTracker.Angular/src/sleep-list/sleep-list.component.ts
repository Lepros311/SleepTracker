import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SleepService } from '../app/services/sleep';
import { SleepReadDto } from '../app/models/sleep-read-dto';
import { PagedResponse } from '../app/models/paged-response';
import { SleepEditDialogComponent } from './sleep-edit-dialog.component';
import { SleepDeleteDialogComponent } from './sleep-delete-dialog.component';
import { SleepCreateDialogComponent } from './sleep-create-dialog.component';

@Component({
  selector: 'app-sleep-list',
  standalone: true,
  imports: [
    CommonModule, 
    MatButtonModule, 
    MatIconModule, 
    MatDialogModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatSnackBarModule
  ],
  templateUrl: './sleep-list.component.html',
  styleUrl: './sleep-list.component.css'
})
export class SleepListComponent implements OnInit, OnDestroy {
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

  filterStartDate = signal<Date | null>(null);
  filterEndDate = signal<Date | null>(null);

  constructor(
    private sleepService: SleepService, 
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void { this.loadSleeps(); }

  ngOnDestroy(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }

  loadSleeps(): void {
    this.loading.set(true);
    this.error.set(null);

    this.sleepService.getSleeps(
      this.pageNumber(), 
      this.pageSize(),
      this.filterStartDate() || undefined,
      this.filterEndDate() || undefined
    ).subscribe({
      next: (response: PagedResponse<SleepReadDto[]>) => {
        if (response.status === 0 && response.data) {
          this.sleeps.set(response.data);
          this.totalRecords.set(response.totalRecords);
          this.totalPages.set(response.totalPages);
        } else {
          this.error.set(response.message || 'Failed to load sleep records');
          this.snackBar.open('Failed to load sleep records', 'Close', {
            duration: 3000,
            horizontalPosition: 'center',
            verticalPosition: 'bottom'
          });
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load sleep records. Please try again.');
        this.loading.set(false);
        console.error('Error loading sleeps:', err);
        this.snackBar.open('Failed to load sleep records. Please try again.', 'Close', {
          duration: 3000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom'
        });
      }
    });
  }

  onFilterChange(): void {
    this.pageNumber.set(1); // Reset to first page when filter changes
    this.loadSleeps();
  }

  clearFilters(): void {
    this.filterStartDate.set(null);
    this.filterEndDate.set(null);
    this.pageNumber.set(1);
    this.loadSleeps();
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
        this.elapsedTime.set('00:00:00');
        this.loadSleeps();
        this.snackBar.open('Sleep record created successfully', 'Close', {
          duration: 3000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom'
        });
      },
      error: (err) => {
        console.error('Error creating sleep:', err);
        this.snackBar.open('Failed to create sleep record. Please try again.', 'Close', {
          duration: 3000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom'
        });
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
        this.snackBar.open('Sleep record created successfully', 'Close', {
          duration: 3000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom'
        });
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
        this.snackBar.open('Sleep record updated successfully', 'Close', {
          duration: 3000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom'
        });
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
            this.snackBar.open('Sleep record deleted successfully', 'Close', {
              duration: 3000,
              horizontalPosition: 'center',
              verticalPosition: 'bottom'
            });
          },
          error: (err) => {
            console.error('Error deleting sleep:', err);
            this.snackBar.open('Failed to delete sleep record. Please try again.', 'Close', {
              duration: 3000,
              horizontalPosition: 'center',
              verticalPosition: 'bottom'
            });
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

  formatDuration(start: string, end: string): string {
    const startDate = new Date(start);
    const endDate = new Date(end);
    const diffMs = endDate.getTime() - startDate.getTime();
    const diffSeconds = Math.floor(diffMs / 1000);
    const diffMinutes = Math.floor(diffSeconds / 60);
    const hours = Math.floor(diffMinutes / 60);
    const minutes = diffMinutes % 60;
    const seconds = diffSeconds % 60;

    if (diffSeconds < 60) {
      return `${seconds} second${seconds !== 1 ? 's' : ''}`;
    } else if (hours === 0) {
      return `${minutes} minute${minutes !== 1 ? 's' : ''}`;
    } else if (minutes === 0) {
      return `${hours} hour${hours !== 1 ? 's' : ''}`;
    } else {
      return `${hours} hour${hours !== 1 ? 's' : ''} and ${minutes} minute${minutes !== 1 ? 's' : ''}`;
    }
  }
}