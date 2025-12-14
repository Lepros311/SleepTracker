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

  hours = Array.from({ length: 24 }, (_, i) => i);
  minutes = Array.from({ length: 60 }, (_, i) => i);

  constructor(
    private fb: FormBuilder,
    private sleepService: SleepService,
    private dialogRef: MatDialogRef<SleepEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { sleep: SleepReadDto }
  ) {
    const startDate = new Date(data.sleep.start);
    const endDate = new Date(data.sleep.end);

    this.editForm = this.fb.group({
      startDate: [startDate, Validators.required],
      startHour: [startDate.getHours(), Validators.required],
      startMinute: [startDate.getMinutes(), Validators.required],
      endDate: [endDate, Validators.required],
      endHour: [endDate.getHours(), Validators.required],
      endMinute: [endDate.getMinutes(), Validators.required]
    });
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
      
      const startDateTime = this.combineDateTime(
        formValue.startDate, 
        formValue.startHour, 
        formValue.startMinute
      );
      const endDateTime = this.combineDateTime(
        formValue.endDate, 
        formValue.endHour, 
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