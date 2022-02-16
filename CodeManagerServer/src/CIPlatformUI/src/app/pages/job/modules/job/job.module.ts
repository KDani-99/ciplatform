import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../../../shared/shared.module';
import { RunModule } from '../../../run/modules/run/run.module';

@NgModule({
  declarations: [],
  imports: [CommonModule, SharedModule, RunModule],
})
export class JobModule {}
