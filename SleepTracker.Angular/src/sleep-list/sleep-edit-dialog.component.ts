import { Component, Inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { SleepService } from '../app/services/sleep';
import { SleepReadDto } from '../app/models/sleep-read-dto';

@Component({
  selector: 'app-sleep-edit-dialog',
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
  templateUrl: './sleep-edit-dialog.component.html',
  styleUrl: './sleep-edit-dialog.component.css'
})
export class SleepEditDialogComponent {
  editForm: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);

  hours = Array.from({ length: 12 }, (_, i) => i);
  minutes = Array.from({ length: 60 }, (_, i) => i);
  periods = ['AM', 'PM'];

  constructor(
    private fb: FormBuilder,
    private sleepService: SleepService,
    private dialogRef: MatDialogRef<SleepEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { sleep: SleepReadDto }
  ) {
    const startDate = new Date(data.sleep.start);
    const endDate = new Date(data.sleep.end);

    const startHour24 = startDate.getHours();
    const endHour24 = endDate.getHours();

    this.editForm = this.fb.group({
      startDate: [startDate, Validators.required],
      startHour: [this.to12Hour(startHour24), Validators.required],
      startMinute: [startDate.getMinutes(), Validators.required],
      startPeriod: [startHour24 >= 12 ? 'PM' : 'AM', Validators.required],
      endDate: [endDate, Validators.required],
      endHour: [this.to12Hour(endHour24), Validators.required],
      endMinute: [endDate.getMinutes(), Validators.required],
      endPeriod: [endHour24 >= 12 ? 'PM' : 'AM', Validators.required]
    });
  }

  to12Hour(hour24: number): number {
    if (hour24 === 0) return 12;
    if (hour24 > 12) return hour24 - 12;
    return hour24;
  }

  to24Hour(hour12: number, period: string): number {
    if (period === 'AM') {
      return hour12 === 12 ? 0 : hour12;
    } else {
      return hour12 === 12 ? 12 : hour12 + 12;
    }
  }

  formatHour(hour: number): string {
    return hour.toString().padStart(2, '0');
  }

  formatMinute(minute: number): string {
    return minute.toString().padStart(2, '0');
  }

  onSubmit(): void {
    if (this.editForm.valid) {
      this.loading.set(true);
      this.error.set(null);

      const formValue = this.editForm.value;

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

      const updateDto = {
        start: startDateTime,
        end: endDateTime
      };

      this.sleepService.updateSleep(this.data.sleep.id, updateDto).subscribe({
        next: () => {
          this.dialogRef.close('updated');
        },
        error: (err) => {
          this.error.set('Failed to update sleep record. Please try again.');
          this.loading.set(false);
          console.error('Error updating sleep:', err);
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