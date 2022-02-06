import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../../shared/shared.module';
import { TeamModule } from '../../../team/modules/team/team.module';
import { EditComponent } from './components/edit/edit.component';
import { DeleteComponent } from './components/delete/delete.component';

@NgModule({
  declarations: [EditComponent, DeleteComponent],
  imports: [CommonModule, SharedModule, TeamModule],
  exports: [EditComponent, DeleteComponent],
})
export class ProjectModule {}
