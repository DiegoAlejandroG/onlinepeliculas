import type ProductoraCreacion from "../modelos/ProductoraCreacion.model";

export default function convertirProductoraAFormData(productora: ProductoraCreacion): FormData {
    const formData = new FormData();
    formData.append("nombre", productora.nombre);
    formData.append("fechaFundacion", productora.fechaFundacion);
    formData.append("sitioWeb", productora.sitioWeb);

    if (productora.logo instanceof File) {
        formData.append("logo", productora.logo);
    }

    return formData;
}
