import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../../shared/shared.module';
import { TeamModule } from '../../../team/modules/team/team.module';
import { EditComponent } from './components/edit/edit.component';
import { DeleteComponent } from './components/delete/delete.component';
import { VariablesComponent } from './components/variables/variables.component';

@NgModule({
  declarations: [EditComponent, DeleteComponent, VariablesComponent],
  imports: [CommonModule, SharedModule, TeamModule],
  exports: [EditComponent, DeleteComponent, VariablesComponent],
})
export class ProjectModule {}
