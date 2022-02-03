import { Component, Input, OnInit } from '@angular/core';
import { ProjectDto } from '../../../../project/project.interface';

@Component({
  selector: 'projects-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss'],
})
export class ProjectComponent implements OnInit {
  @Input() project?: ProjectDto;

  constructor() {}

  ngOnInit(): void {}
}
