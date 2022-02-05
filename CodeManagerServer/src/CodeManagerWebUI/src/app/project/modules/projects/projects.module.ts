import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectComponent } from './components/project/project.component';
import { SharedModule } from '../../../shared/shared.module';
import { CreateProjectComponent } from '../../../team/modules/team/components/create-project/create-project.component';

@NgModule({
  declarations: [ProjectComponent, CreateProjectComponent],
  imports: [CommonModule, SharedModule],
  exports: [ProjectComponent, CreateProjectComponent],
})
export class ProjectsModule {}
