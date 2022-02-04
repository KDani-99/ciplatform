import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataComponent } from '../../shared/data/data.component';
import { ProjectComponent } from './components/project/project.component';
import { MemberComponent } from './components/member/member.component';
import { ContentComponent } from '../../shared/content/content.component';
import { SharedModule } from '../../shared/shared.module';
import { EditComponent } from './components/edit/edit.component';

@NgModule({
  declarations: [
    DataComponent,
    ProjectComponent,
    MemberComponent,
    ContentComponent,
    EditComponent,
  ],
  exports: [DataComponent, MemberComponent, ContentComponent, EditComponent],
  imports: [CommonModule, SharedModule],
})
export class TeamModule {}
