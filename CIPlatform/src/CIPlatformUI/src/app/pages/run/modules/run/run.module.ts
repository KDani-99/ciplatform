import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConsoleComponent } from '../../../job/modules/job/components/console/console.component';
import { SharedModule } from '../../../../shared/shared.module';
import { DeleteComponent } from './components/delete.component';

@NgModule({
  declarations: [ConsoleComponent, DeleteComponent],
  exports: [ConsoleComponent, DeleteComponent],
  imports: [CommonModule, SharedModule],
})
export class RunModule {}
