import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-run',
  templateUrl: './run.component.html',
  styleUrls: ['./run.component.scss'],
})
export class RunComponent implements OnInit {
  constructor(private readonly router: Router) {}

  ngOnInit(): void {}

  async openJob(id: number): Promise<void> {
    await this.router.navigate([`jobs/${id}`]);
  }
}
