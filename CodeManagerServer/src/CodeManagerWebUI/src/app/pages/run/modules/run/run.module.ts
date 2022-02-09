import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConsoleComponent } from '../../../job/modules/job/components/console/console.component';
import { SharedModule } from '../../../../shared/shared.module';

@NgModule({
  declarations: [ConsoleComponent],
  exports: [ConsoleComponent],
  imports: [CommonModule, SharedModule],
})
export class RunModule {}
