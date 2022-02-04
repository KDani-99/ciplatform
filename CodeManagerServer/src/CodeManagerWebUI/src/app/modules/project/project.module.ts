import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';
import { TeamModule } from '../team/team.module';
import { EditComponent } from './components/edit/edit.component';

@NgModule({
  declarations: [EditComponent],
  imports: [CommonModule, SharedModule, TeamModule],
  exports: [EditComponent],
})
export class ProjectModule {}
