import { Component, Input, OnInit } from '@angular/core';
import { ProjectDto } from '../../project.interface';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../project.service';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss'],
})
export class ProjectComponent implements OnInit {
  @Input() project?: ProjectDto;
  showEditProjectWindow: boolean = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly projectService: ProjectService,
  ) {}

  ngOnInit(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.projectService.getProject(id).subscribe({
      next: (project: ProjectDto) => {
        this.project = project;
      },
    });
  }

  toggleEditProjectWindow(show: boolean): void {
    this.showEditProjectWindow = show;
  }

  async openRun(id: number): Promise<void> {
    await this.router.navigate([`runs/${id}`]);
  }
}
