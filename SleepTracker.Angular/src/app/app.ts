import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SleepService } from './services/sleep';
import { SleepListComponent } from '../sleep-list/sleep-list.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SleepListComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Sleep Tracker');

  constructor(private sleepService: SleepService) {}

  ngOnInit() {
    this.sleepService.getSleeps().subscribe({
      next: (data) => console.log('API data:', data),
      error: (err) => console.error('API error:', err),
    });
  }
}
