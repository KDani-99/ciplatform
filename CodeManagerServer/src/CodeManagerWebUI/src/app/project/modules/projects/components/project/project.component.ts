import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ProjectDto } from '../../../../project.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'projects-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss'],
})
export class ProjectComponent implements OnInit {
  @Input() project?: ProjectDto;

  constructor(private readonly router: Router) {}

  ngOnInit(): void {}

  async open(): Promise<void> {
    await this.router.navigate([`projects/${this.project?.id}`]);
  }
}
