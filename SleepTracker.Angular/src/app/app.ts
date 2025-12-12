import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SleepService } from './services/sleep';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('SleepTracker.Angular');

  constructor(private sleepService: SleepService) {}

  ngOnInit() {
    this.sleepService.getSleeps().subscribe({
      next: (data) => console.log('API data:', data),
      error: (err) => console.error('API error:', err),
    });
  }
}
