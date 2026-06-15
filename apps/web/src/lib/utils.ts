import { type ClassValue, clsx } from 'clsx'
import { twMerge } from 'tailwind-merge'

/** Gộp class Tailwind an toàn (chuẩn shadcn-vue). */
export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}
