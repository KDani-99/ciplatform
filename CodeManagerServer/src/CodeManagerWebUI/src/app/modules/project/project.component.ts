import { Component, Input, OnInit } from '@angular/core';
import { ProjectDto } from '../../project/project.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss'],
})
export class ProjectComponent implements OnInit {
  @Input() project?: ProjectDto;
  showEditProjectWindow: boolean = false;

  constructor(private readonly router: Router) {}

  ngOnInit(): void {}

  toggleEditProjectWindow(show: boolean): void {
    this.showEditProjectWindow = show;
  }

  async openRun(id: number): Promise<void> {
    await this.router.navigate([`runs/${id}`]);
  }
}
