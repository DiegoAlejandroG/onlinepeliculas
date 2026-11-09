export default function Boton(props: BotonProps){

    return (
        <button type={props.type ?? 'button'}
            className={props.className ?? 'btn btn-primary'}
            disabled={props.disabled}
            onClick={props.onClick}>
            {props.children}
        </button>
    )

}

interface BotonProps{
    children: React.ReactNode;
    type?: 'button' | 'submit' | 'reset';
    disabled?: boolean;
    className?: string;
    onClick?: () => void;
}
