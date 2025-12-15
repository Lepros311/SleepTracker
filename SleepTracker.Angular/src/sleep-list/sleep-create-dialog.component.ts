import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { SleepService } from '../app/services/sleep';

@Component({
  selector: 'app-sleep-create-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    MatSelectModule
  ],
  templateUrl: './sleep-create-dialog.component.html',
  styleUrl: './sleep-create-dialog.component.css'
})
export class SleepCreateDialogComponent {
  createForm: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);

  // 12-hour format: 1-12
  hours = Array.from({ length: 12 }, (_, i) => i + 1);
  minutes = Array.from({ length: 60 }, (_, i) => i);
  periods = ['AM', 'PM'];

  constructor(
    private fb: FormBuilder,
    private sleepService: SleepService,
    private dialogRef: MatDialogRef<SleepCreateDialogComponent>
  ) {
    this.createForm = this.fb.group({
      startDate: [null, Validators.required],
      startHour: [null, Validators.required],
      startMinute: [null, Validators.required],
      startPeriod: [null, Validators.required],
      endDate: [null, Validators.required],
      endHour: [null, Validators.required],
      endMinute: [null, Validators.required],
      endPeriod: [null, Validators.required]
    });
  }

  // Convert 24-hour (0-23) to 12-hour (1-12)
  to12Hour(hour24: number): number {
    if (hour24 === 0) return 12;
    if (hour24 > 12) return hour24 - 12;
    return hour24;
  }

  // Convert 12-hour format back to 24-hour format
  to24Hour(hour12: number, period: string): number {
    if (period === 'AM') {
      return hour12 === 12 ? 0 : hour12;
    } else {
      return hour12 === 12 ? 12 : hour12 + 12;
    }
  }

  formatHour(hour: number): string {
    return hour.toString();
  }

  formatMinute(minute: number): string {
    return minute.toString().padStart(2, '0');
  }

  onSubmit(): void {
    if (this.createForm.valid) {
      this.loading.set(true);
      this.error.set(null);

      const formValue = this.createForm.value;
      
      // Convert 12-hour format to 24-hour format
      const startHour24 = this.to24Hour(formValue.startHour, formValue.startPeriod);
      const endHour24 = this.to24Hour(formValue.endHour, formValue.endPeriod);
      
      const startDateTime = this.combineDateTime(
        formValue.startDate, 
        startHour24, 
        formValue.startMinute
      );
      const endDateTime = this.combineDateTime(
        formValue.endDate, 
        endHour24, 
        formValue.endMinute
      );

      const createDto = {
        start: startDateTime,
        end: endDateTime
      };

      this.sleepService.createSleep(createDto).subscribe({
        next: () => {
          this.dialogRef.close('created');
        },
        error: (err) => {
          this.error.set('Failed to create sleep record. Please try again.');
          this.loading.set(false);
          console.error('Error creating sleep:', err);
        }
      });
    }
  }

  combineDateTime(date: Date, hour: number, minute: number): string {
    const combined = new Date(date);
    combined.setHours(hour);
    combined.setMinutes(minute);
    combined.setSeconds(0);
    combined.setMilliseconds(0);
    return combined.toISOString();
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}