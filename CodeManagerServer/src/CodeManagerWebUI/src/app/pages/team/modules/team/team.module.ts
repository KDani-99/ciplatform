import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataComponent } from '../../../../shared/data/data.component';
import { MemberComponent } from './components/member/member.component';
import { ContentComponent } from '../../../../shared/content/content.component';
import { SharedModule } from '../../../../shared/shared.module';
import { EditComponent } from './components/edit/edit.component';
import { DeleteComponent } from './components/delete/delete.component';
import { KickComponent } from './components/kick/kick.component';

@NgModule({
  declarations: [
    DataComponent,
    MemberComponent,
    ContentComponent,
    EditComponent,
    DeleteComponent,
    KickComponent,
  ],
  exports: [
    DataComponent,
    MemberComponent,
    ContentComponent,
    EditComponent,
    DeleteComponent,
    KickComponent,
  ],
  imports: [CommonModule, SharedModule],
})
export class TeamModule {}
