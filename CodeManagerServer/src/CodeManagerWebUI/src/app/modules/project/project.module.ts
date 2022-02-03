import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectComponent } from './project.component';
import { SharedModule } from '../../shared/shared.module';
import { TeamModule } from '../team/team.module';

@NgModule({
  declarations: [ProjectComponent],
  imports: [CommonModule, SharedModule, TeamModule],
  exports: [ProjectComponent],
})
export class ProjectModule {}
