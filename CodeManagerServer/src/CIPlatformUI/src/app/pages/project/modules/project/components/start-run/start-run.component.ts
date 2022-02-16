import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { CheckboxComponent } from '../../../../../../shared/checkbox/checkbox.component';
import { InputComponent } from '../../../../../../shared/input/input.component';
import { ProjectService } from '../../../../../../services/project/project.service';
import { RunService } from '../../../../../../services/run/run.service';

@Component({
  selector: 'start-run',
  templateUrl: './start-run.component.html',
  styleUrls: ['./start-run.component.scss'],
})
export class StartRunComponent implements OnInit {
  instructionsFile: File | null = null;

  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  @Output() onStart: EventEmitter<File | null> =
    new EventEmitter<File | null>();

  constructor() {}

  ngOnInit(): void {}

  close(): void {
    this.onClose.emit();
  }

  start(): void {
    this.onStart.emit(this.instructionsFile);
  }

  onFileChange(e: any) {
    console.log(e.target.files.item(0));
    this.instructionsFile = e.target.files.item(0);
  }
}
