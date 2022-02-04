import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss'],
})
export class ProjectsComponent implements OnInit {
  showCreateProjectWindow: boolean = false;

  constructor() {}

  ngOnInit(): void {}

  toggleCreateProjectsWindow(show: boolean): void {
    this.showCreateProjectWindow = show;
  }
}
